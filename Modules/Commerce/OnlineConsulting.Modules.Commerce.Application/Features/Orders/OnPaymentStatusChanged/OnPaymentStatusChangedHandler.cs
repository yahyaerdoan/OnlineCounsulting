using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Contracts;
using OnlineConsulting.SharedKernel.Payments;
using OrderPaymentStatuses = OnlineConsulting.Modules.Commerce.Application.Features.Orders.Contracts.PaymentStatuses;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Orders.OnPaymentStatusChanged;

/// <summary>PaymentSucceededNotification/PaymentFailedNotification are published for every reference id across every module (Order, Appointment, ...) - ReferenceId not parsing as a Guid, or not matching one of this tenant's orders, just means the event belongs to a different module's handler, not an error here.</summary>
public class OnPaymentStatusChangedHandler(IOrderRepository orderRepository) :
    INotificationHandler<PaymentSucceededNotification>,
    INotificationHandler<PaymentFailedNotification>
{
    public Task Handle(PaymentSucceededNotification notification, CancellationToken cancellationToken) =>
        UpdateOrderPaymentStatusAsync(notification.ReferenceId, notification.ProviderPaymentId, OrderPaymentStatuses.Paid, cancellationToken);

    public Task Handle(PaymentFailedNotification notification, CancellationToken cancellationToken) =>
        UpdateOrderPaymentStatusAsync(notification.ReferenceId, notification.ProviderPaymentId, OrderPaymentStatuses.Cancelled, cancellationToken);

    private async Task UpdateOrderPaymentStatusAsync(string referenceId, string providerPaymentId, string newPaymentStatus, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(referenceId, out var orderId))
            return;

        var order = await orderRepository.GetAsync(o => o.Id == orderId && o.ProviderPaymentId == providerPaymentId, cancellationToken: cancellationToken);
        if (order is null)
            return;

        order.PaymentStatus = newPaymentStatus;
        await orderRepository.UpdateAsync(order);
    }
}
