using MediatR;
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

namespace OnlineConsulting.Modules.Commerce.Application.Features.Orders.OnPaymentStatusChanged;

/// <summary>These notifications fire for every module's reference ids - a ReferenceId that doesn't parse or match one of this tenant's orders just belongs to another module's handler, not an error.</summary>
public class OnPaymentStatusChangedHandler(IOrderRepository orderRepository, IOrderItemRepository orderItemRepository, IUserContactReader userContactReader, IBasketRepository basketRepository, IBasketItemRepository basketItemRepository, IEmailOutboxWriter<ICommerceOutboxModule> outboxWriter, IEmailTemplate<OrderConfirmationEmailModel> confirmationTemplate, IEmailTemplate<OrderPaymentFailedEmailModel> paymentFailedTemplate) :
    INotificationHandler<PaymentSucceededNotification>, INotificationHandler<PaymentFailedNotification>
{
    /// <summary>
    /// Marks the order Paid and clears its basket - the basket is deliberately left alone by
    /// <c>CreateOrderFromBasketHandler</c> for an async/Pending result, so this webhook confirming
    /// the charge is what actually clears it.
    /// </summary>
    public async Task Handle(PaymentSucceededNotification notification, CancellationToken cancellationToken)
    {
        var order = await UpdateOrderPaymentStatusAsync(notification.ReferenceId, notification.ProviderPaymentId, OrderPaymentStatuses.Paid, cancellationToken);

        if (order is null)
        {
            return;
        }

        await ClearBasketAsync(order.UserId, cancellationToken);

        var email = await userContactReader.GetEmailAsync(order.UserId, cancellationToken);

        if (email is null)
        {
            return;
        }

        var items = await orderItemRepository.GetListAsync(i => i.OrderId == order.Id, orderBy: q => q.OrderBy(i => i.Id), size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);
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

        var basketItems = await basketItemRepository.GetListAsync(i => i.BasketId == basket.Id, orderBy: q => q.OrderBy(i => i.Id), size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);

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
