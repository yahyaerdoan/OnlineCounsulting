using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Abstractions;
using OnlineConsulting.Modules.Commerce.Domain;
using OnlineConsulting.SharedKernel.Payments;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Orders.OnPaymentStatusChanged;

public class OnPaymentStatusChangedHandler(IOrderRepository orderRepository,
                                           IOrderFulfillment fulfillment,
                                           IOrderNotifier notifier) :
    INotificationHandler<PaymentSucceededNotification>, INotificationHandler<PaymentFailedNotification>
{
    public async Task Handle(PaymentSucceededNotification notification, CancellationToken cancellationToken)
    {
        if (await SettleAsync(notification.ReferenceId, notification.ProviderPaymentId, o => o.MarkPaid(), cancellationToken) is { } order)
        {
            await fulfillment.CompletePaidAsync(order, cancellationToken);
        }
    }

    public async Task Handle(PaymentFailedNotification notification, CancellationToken cancellationToken)
    {
        if (await SettleAsync(notification.ReferenceId, notification.ProviderPaymentId, o => o.FailPayment(), cancellationToken) is { } order)
        {
            await notifier.PaymentFailedAsync(order, cancellationToken);
        }
    }

    private async Task<Order?> SettleAsync(string referenceId, string providerPaymentId, Action<Order> settle, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(referenceId, out var orderId))
        {
            return null;
        }

        var order = await orderRepository.GetAsync(o => o.Id == orderId && o.ProviderPaymentId == providerPaymentId, cancellationToken: cancellationToken);
        if (order is null || !order.IsAwaitingPayment)
        {
            return null;
        }

        settle(order);

        _ = await orderRepository.UpdateAsync(order, cancellationToken: cancellationToken);
        return order;
    }
}
