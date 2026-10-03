using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Abstractions;
using OnlineConsulting.Modules.Commerce.Domain;
using OnlineConsulting.SharedKernel.Payments;
using OrderPaymentStatuses = OnlineConsulting.Modules.Commerce.Application.Features.Orders.Constants.PaymentStatuses;
using OrderStatuses = OnlineConsulting.Modules.Commerce.Application.Features.Orders.Constants.OrderStatuses;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Orders.OnPaymentStatusChanged;

/// <summary>
/// Settles an order from a payment outcome, whoever reported it: the provider's webhook, the payment page's reconciliation
/// (ResumeOrderPayment) or the background cleanup. These notifications fire for every module's reference ids - a ReferenceId that
/// doesn't match one of this tenant's Pending orders belongs to another handler (or was already settled) and is ignored.
/// </summary>
public class OnPaymentStatusChangedHandler(IOrderRepository orderRepository, IOrderFulfillment fulfillment, IOrderNotifier notifier) :
    INotificationHandler<PaymentSucceededNotification>, INotificationHandler<PaymentFailedNotification>
{
    public async Task Handle(PaymentSucceededNotification notification, CancellationToken cancellationToken)
    {
        if (await SettleAsync(notification.ReferenceId, notification.ProviderPaymentId, OrderPaymentStatuses.Paid, cancellationToken) is { } order)
        {
            await fulfillment.CompletePaidAsync(order, cancellationToken);
        }
    }

    public async Task Handle(PaymentFailedNotification notification, CancellationToken cancellationToken)
    {
        if (await SettleAsync(notification.ReferenceId, notification.ProviderPaymentId, OrderPaymentStatuses.Cancelled, cancellationToken) is { } order)
        {
            await notifier.PaymentFailedAsync(order, cancellationToken);
        }
    }

    private async Task<Order?> SettleAsync(string referenceId, string providerPaymentId, string newPaymentStatus, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(referenceId, out var orderId))
        {
            return null;
        }

        var order = await orderRepository.GetAsync(o => o.Id == orderId && o.ProviderPaymentId == providerPaymentId, cancellationToken: cancellationToken);
        if (order is null || order.PaymentStatus != OrderPaymentStatuses.Pending)
        {
            return null;
        }

        order.PaymentStatus = newPaymentStatus;
        if (newPaymentStatus == OrderPaymentStatuses.Cancelled)
        {
            order.OrderStatus = OrderStatuses.Cancelled;
        }

        _ = await orderRepository.UpdateAsync(order);
        return order;
    }
}
