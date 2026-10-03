using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Abstractions;
using OnlineConsulting.Modules.Commerce.Domain;
using OnlineConsulting.SharedKernel.Payments;
using OnlineConsulting.SharedKernel.Persistence;
using OrderPaymentStatuses = OnlineConsulting.Modules.Commerce.Application.Features.Orders.Constants.PaymentStatuses;
using OrderStatuses = OnlineConsulting.Modules.Commerce.Application.Features.Orders.Constants.OrderStatuses;

namespace OnlineConsulting.Modules.Commerce.Infrastructure.Cleanup;

/// <summary>Reconciles Pending orders against the payment provider (catches lost webhooks) and cancels ones abandoned past ExpireAfter.</summary>
public class PendingOrderCleanupService(IServiceScopeFactory scopeFactory, IOptions<PendingOrderCleanupOptions> options, ILogger<PendingOrderCleanupService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var settings = options.Value;

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CleanupOnceAsync(settings, stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Pending order cleanup cycle failed unexpectedly.");
            }

            await Task.Delay(settings.PollInterval, stoppingToken);
        }
    }

    private async Task CleanupOnceAsync(PendingOrderCleanupOptions settings, CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();

        var reconcileCutoff = DateTimeOffset.UtcNow - settings.ReconcileAfter;

        var candidates = await orderRepository
            .GetListAsync(predicate: o => o.PaymentStatus == OrderPaymentStatuses.Pending && o.CreatedDate <= reconcileCutoff, orderBy: q => q.OrderBy(o => o.CreatedDate), size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);

        if (candidates.Items.Count == 0)
        {
            return;
        }

        var expireCutoff = DateTimeOffset.UtcNow - settings.ExpireAfter;
        var reconciledCount = 0;
        var expiredCount = 0;

        foreach (var order in candidates.Items)
        {
            if (await TryReconcileAsync(scope.ServiceProvider, order, cancellationToken))
            {
                reconciledCount++;
                continue;
            }

            if (order.CreatedDate <= expireCutoff)
            {
                order.PaymentStatus = OrderPaymentStatuses.Cancelled;
                order.OrderStatus = OrderStatuses.Cancelled;

                _ = await orderRepository.UpdateAsync(order);

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
            logger.LogInformation("Pending order cleanup reconciled {ReconciledCount} and expired {ExpiredCount} of {CandidateCount} candidate order(s).", reconciledCount, expiredCount, candidates.Items.Count);
        }
    }

    /// <summary>
    /// Returns true if the provider already settled the payment (succeeded or failed), so the caller skips the expiry check. The outcome is
    /// published exactly like the provider's webhook would, so OnPaymentStatusChangedHandler settles the order, issues the receipt and
    /// notifies the customer the same way whichever path got there first.
    /// </summary>
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
