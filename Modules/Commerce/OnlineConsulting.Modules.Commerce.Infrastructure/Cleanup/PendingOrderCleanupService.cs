using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OnlineConsulting.Modules.Commerce.Application.Common.Templates;
using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Abstractions;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Abstractions;
using OnlineConsulting.Modules.Commerce.Domain;
using OnlineConsulting.SharedKernel.Identity;
using OnlineConsulting.SharedKernel.Notifications;
using OnlineConsulting.SharedKernel.Notifications.Templates;
using OnlineConsulting.SharedKernel.Payments;
using OnlineConsulting.SharedKernel.Persistence;
using OrderPaymentStatuses = OnlineConsulting.Modules.Commerce.Application.Features.Orders.Contracts.PaymentStatuses;
using OrderStatuses = OnlineConsulting.Modules.Commerce.Application.Features.Orders.Contracts.OrderStatuses;

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

                await NotifyAsync(scope.ServiceProvider, order, isAbandoned: true, cancellationToken);
            }
        }

        if ((reconciledCount > 0 || expiredCount > 0) && logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Pending order cleanup reconciled {ReconciledCount} and expired {ExpiredCount} of {CandidateCount} candidate order(s).", reconciledCount, expiredCount, candidates.Items.Count);
        }
    }

    /// <summary>
    /// Returns true if the order's status was resolved (Paid or Cancelled) so the caller skips the expiry check for it.
    /// A Paid result runs the basket-clear/confirmation steps directly, since it means the webhook that
    /// would normally trigger them (<c>OnPaymentStatusChangedHandler</c>) never arrived.
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

        var orderRepository = serviceProvider.GetRequiredService<IOrderRepository>();

        if (status.Status == PaymentStatuses.Succeeded)
        {
            order.PaymentStatus = OrderPaymentStatuses.Paid;

            _ = await orderRepository.UpdateAsync(order);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Reconciled order {OrderId} ({OrderNumber}) as Paid - a webhook for this payment was never received.", order.Id, order.OrderNumber);
            }

            await ClearBasketAndSendConfirmationAsync(serviceProvider, order, cancellationToken);

            return true;
        }

        if (status.Status == PaymentStatuses.Failed)
        {
            order.PaymentStatus = OrderPaymentStatuses.Cancelled;
            order.OrderStatus = OrderStatuses.Cancelled;

            _ = await orderRepository.UpdateAsync(order);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Reconciled order {OrderId} ({OrderNumber}) as Cancelled - the payment failed at the provider.", order.Id, order.OrderNumber);
            }

            await NotifyAsync(serviceProvider, order, isAbandoned: false, cancellationToken);

            return true;
        }

        return false;
    }

    private static async Task ClearBasketAndSendConfirmationAsync(IServiceProvider serviceProvider, Order order, CancellationToken cancellationToken)
    {
        var basketRepository = serviceProvider.GetRequiredService<IBasketRepository>();
        var basketItemRepository = serviceProvider.GetRequiredService<IBasketItemRepository>();

        var basket = await basketRepository.GetAsync(b => b.UserId == order.UserId, cancellationToken: cancellationToken);

        if (basket is not null)
        {
            var basketItems = await basketItemRepository.GetListAsync(i => i.BasketId == basket.Id, orderBy: q => q.OrderBy(i => i.Id), size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);

            foreach (var basketItem in basketItems.Items)
            {
                _ = await basketItemRepository.DeleteAsync(basketItem);
            }

            _ = await basketRepository.DeleteAsync(basket);
        }

        var userContactReader = serviceProvider.GetRequiredService<IUserContactReader>();
        var email = await userContactReader.GetEmailAsync(order.UserId, cancellationToken);

        if (email is null)
        {
            return;
        }

        var orderItemRepository = serviceProvider.GetRequiredService<IOrderItemRepository>();

        var orderItems = await orderItemRepository.GetListAsync(i => i.OrderId == order.Id, orderBy: q => q.OrderBy(i => i.Id), size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);

        var total = orderItems.Items.Sum(i => i.TotalPrice);

        var outboxWriter = serviceProvider.GetRequiredService<IEmailOutboxWriter<ICommerceOutboxModule>>();
        var template = serviceProvider.GetRequiredService<IEmailTemplate<OrderConfirmationEmailModel>>();
        var model = new OrderConfirmationEmailModel(order.OrderNumber, orderItems.Items.Count, total);

        await outboxWriter.EnqueueAsync(email, template.Subject(model), template.Build(model), sourceReference: $"Order:{order.Id}", cancellationToken: cancellationToken);
    }

    /// <summary>isAbandoned picks the tone: a checkout that timed out with no provider response vs. one the provider explicitly failed.</summary>
    private static async Task NotifyAsync(IServiceProvider serviceProvider, Order order, bool isAbandoned, CancellationToken cancellationToken)
    {
        var userContactReader = serviceProvider.GetRequiredService<IUserContactReader>();

        var email = await userContactReader.GetEmailAsync(order.UserId, cancellationToken);

        if (email is null)
        {
            return;
        }

        var outboxWriter = serviceProvider.GetRequiredService<IEmailOutboxWriter<ICommerceOutboxModule>>();

        if (isAbandoned)
        {
            var template = serviceProvider.GetRequiredService<IEmailTemplate<OrderAbandonedEmailModel>>();
            var model = new OrderAbandonedEmailModel(order.OrderNumber);

            await outboxWriter.EnqueueAsync(email, template.Subject(model), template.Build(model), sourceReference: $"Order:{order.Id}", cancellationToken: cancellationToken);
        }
        else
        {
            var template = serviceProvider.GetRequiredService<IEmailTemplate<OrderPaymentFailedEmailModel>>();
            var model = new OrderPaymentFailedEmailModel(order.OrderNumber);

            await outboxWriter.EnqueueAsync(email, template.Subject(model), template.Build(model), sourceReference: $"Order:{order.Id}", cancellationToken: cancellationToken);
        }
    }
}
