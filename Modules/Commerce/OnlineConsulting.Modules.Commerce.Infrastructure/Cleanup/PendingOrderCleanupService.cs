using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Abstractions;
using OnlineConsulting.Modules.Commerce.Domain;
using OnlineConsulting.SharedKernel.Payments;

namespace OnlineConsulting.Modules.Commerce.Infrastructure.Cleanup;

/// <summary>Settles pending orders whose webhook was missed and abandons those older than ExpireAfter.</summary>
public class PendingOrderCleanupService(IServiceScopeFactory scopeFactory, IOptions<PendingOrderCleanupOptions> options, ILogger<PendingOrderCleanupService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var settings = options.Value;
        using var timer = new PeriodicTimer(settings.PollInterval);

        do
        {
            try
            {
                await CleanupOnceAsync(settings, stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Pending order cleanup cycle failed unexpectedly.");
            }
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }

    private async Task CleanupOnceAsync(PendingOrderCleanupOptions settings, CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();

        var reconcileCutoff = DateTimeOffset.UtcNow - settings.ReconcileAfter;

        var candidates = await orderRepository
            .GetAllAsync(predicate: o => o.PaymentStatus == OrderPaymentStatuses.Pending && o.OrderStatus != OrderStatuses.Cancelled && o.CreatedDate <= reconcileCutoff, orderBy: q => q.OrderBy(o => o.CreatedDate), cancellationToken: cancellationToken);

        if (candidates.Count == 0)
        {
            return;
        }

        var expireCutoff = DateTimeOffset.UtcNow - settings.ExpireAfter;
        var reconciledCount = 0;
        var expiredCount = 0;

        foreach (var order in candidates)
        {
            if (await TryReconcileAsync(scope.ServiceProvider, order, cancellationToken))
            {
                reconciledCount++;
                continue;
            }

            if (order.CreatedDate <= expireCutoff)
            {
                order.Abandon();

                _ = await orderRepository.UpdateAsync(order, cancellationToken: cancellationToken);

                expiredCount++;

                if (logger.IsEnabled(LogLevel.Information))
                {
                    logger.LogInformation("Cancelled abandoned order {OrderId} ({OrderNumber}) - still Pending after {ExpireAfter}.", order.Id, order.OrderNumber, settings.ExpireAfter);
                }

                await scope.ServiceProvider.GetRequiredService<IOrderNotifier>().AbandonedAsync(order, cancellationToken);
            }
        }

        if ((reconciledCount > 0 || expiredCount > 0) && logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Pending order cleanup reconciled {ReconciledCount} and expired {ExpiredCount} of {CandidateCount} candidate order(s).", reconciledCount, expiredCount, candidates.Count);
        }
    }

    private async Task<bool> TryReconcileAsync(IServiceProvider serviceProvider, Order order, CancellationToken cancellationToken)
    {
        if (order.PaymentProvider is null || order.ProviderPaymentId is null)
        {
            return false;
        }

        var gateway = serviceProvider.GetKeyedService<IPaymentGateway>(order.PaymentProvider);

        if (gateway is null)
        {
            if (logger.IsEnabled(LogLevel.Warning))
            {
                logger.LogWarning("Order {OrderId} references unknown payment provider '{PaymentProvider}' - skipping reconciliation.", order.Id, order.PaymentProvider);
            }

            return false;
        }

        var status = await gateway.GetStatusAsync(order.ProviderPaymentId, cancellationToken);
        var publisher = serviceProvider.GetRequiredService<IPublisher>();

        if (status.Status == PaymentStatuses.Succeeded)
        {
            await publisher.Publish(new PaymentSucceededNotification(order.Id.ToString(), order.ProviderPaymentId, order.PaymentProvider), cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Reconciled order {OrderId} ({OrderNumber}) as Paid - a webhook for this payment was never received.", order.Id, order.OrderNumber);
            }

            return true;
        }

        if (status.Status == PaymentStatuses.Failed)
        {
            await publisher.Publish(new PaymentFailedNotification(order.Id.ToString(), order.ProviderPaymentId, order.PaymentProvider), cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Reconciled order {OrderId} ({OrderNumber}) as Cancelled - the payment failed at the provider.", order.Id, order.OrderNumber);
            }

            return true;
        }

        return false;
    }
}
