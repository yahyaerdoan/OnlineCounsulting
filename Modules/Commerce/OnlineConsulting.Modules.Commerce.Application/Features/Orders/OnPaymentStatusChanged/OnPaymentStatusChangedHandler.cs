using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Common.Templates;
using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Abstractions;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Abstractions;
using OnlineConsulting.SharedKernel.Identity;
using OnlineConsulting.SharedKernel.Notifications;
using OnlineConsulting.SharedKernel.Notifications.Templates;
using OnlineConsulting.SharedKernel.Payments;
using OnlineConsulting.SharedKernel.Persistence;
using OrderStatuses = OnlineConsulting.Modules.Commerce.Application.Features.Orders.Contracts.OrderStatuses;
using OrderPaymentStatuses = OnlineConsulting.Modules.Commerce.Application.Features.Orders.Contracts.PaymentStatuses;
using OnlineConsulting.Modules.Commerce.Domain;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Orders.OnPaymentStatusChanged;

/// <summary>PaymentSucceededNotification/PaymentFailedNotification are published for every reference id across every module (Order, Appointment, ...) - ReferenceId not parsing as a Guid, or not matching one of this tenant's orders, just means the event belongs to a different module's handler, not an error here.</summary>
public class OnPaymentStatusChangedHandler(IOrderRepository orderRepository, IOrderItemRepository orderItemRepository, IUserContactReader userContactReader,
    IBasketRepository basketRepository, IBasketItemRepository basketItemRepository,
    IEmailOutboxWriter<ICommerceOutboxModule> outboxWriter, IEmailTemplate<OrderConfirmationEmailModel> confirmationTemplate,
    IEmailTemplate<OrderPaymentFailedEmailModel> paymentFailedTemplate) :
    INotificationHandler<PaymentSucceededNotification>, INotificationHandler<PaymentFailedNotification>
{
    public async Task Handle(PaymentSucceededNotification notification, CancellationToken cancellationToken)
    {
        var order = await UpdateOrderPaymentStatusAsync(notification.ReferenceId, notification.ProviderPaymentId, OrderPaymentStatuses.Paid, cancellationToken);
        if (order is null)
        {
            return;
        }

        // The basket survived checkout until now (CreateOrderFromBasketHandler leaves it alone for an
        // async/Pending result) - this webhook confirming the charge is what actually clears it.
        await ClearBasketAsync(order.UserId, cancellationToken);

        var email = await userContactReader.GetEmailAsync(order.UserId, cancellationToken);
        if (email is null)
        {
            return;
        }

        var items = await orderItemRepository.GetListAsync(i => i.OrderId == order.Id, size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);
        var total = items.Items.Sum(i => i.TotalPrice);

        var model = new OrderConfirmationEmailModel(order.OrderNumber, items.Items.Count, total);
        await outboxWriter.EnqueueAsync(email, confirmationTemplate.Subject(model), confirmationTemplate.Build(model), sourceReference: $"Order:{order.Id}", cancellationToken: cancellationToken);
    }

    public async Task Handle(PaymentFailedNotification notification, CancellationToken cancellationToken)
    {
        var order = await UpdateOrderPaymentStatusAsync(notification.ReferenceId, notification.ProviderPaymentId, OrderPaymentStatuses.Cancelled, cancellationToken);
        if (order is null)
        {
            return;
        }

        order.OrderStatus = OrderStatuses.Cancelled;
        _ = await orderRepository.UpdateAsync(order);

        var email = await userContactReader.GetEmailAsync(order.UserId, cancellationToken);
        if (email is null)
        {
            return;
        }

        var model = new OrderPaymentFailedEmailModel(order.OrderNumber);
        await outboxWriter.EnqueueAsync(email, paymentFailedTemplate.Subject(model), paymentFailedTemplate.Build(model), sourceReference: $"Order:{order.Id}", cancellationToken: cancellationToken);
    }

    private async Task ClearBasketAsync(Guid userId, CancellationToken cancellationToken)
    {
        var basket = await basketRepository.GetAsync(b => b.UserId == userId, cancellationToken: cancellationToken);
        if (basket is null)
        {
            return;
        }

        var basketItems = await basketItemRepository.GetListAsync(i => i.BasketId == basket.Id, size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);
        foreach (var basketItem in basketItems.Items)
        {
            _ = await basketItemRepository.DeleteAsync(basketItem);
        }

        _ = await basketRepository.DeleteAsync(basket);
    }

    private async Task<Order?> UpdateOrderPaymentStatusAsync(string referenceId, string providerPaymentId, string newPaymentStatus, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(referenceId, out var orderId))
        {
            return null;
        }

        var order = await orderRepository.GetAsync(o => o.Id == orderId && o.ProviderPaymentId == providerPaymentId, cancellationToken: cancellationToken);
        if (order is null)
        {
            return null;
        }

        order.PaymentStatus = newPaymentStatus;
        _ = await orderRepository.UpdateAsync(order);
        return order;
    }
}
