using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Common;
using OnlineConsulting.Modules.Commerce.Application.Common.Templates;
using OnlineConsulting.Modules.Commerce.Application.Features.Addresses.Abstractions;
using OnlineConsulting.Modules.Commerce.Application.Features.Addresses.Constants;
using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Abstractions;
using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Constants;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Abstractions;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Contracts;
using OnlineConsulting.Modules.Commerce.Domain;
using OnlineConsulting.SharedKernel.Notifications;
using OnlineConsulting.SharedKernel.Notifications.Templates;
using OnlineConsulting.SharedKernel.Payments;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;
using OrderPaymentStatuses = OnlineConsulting.Modules.Commerce.Application.Features.Orders.Contracts.PaymentStatuses;
using SharedPaymentStatuses = OnlineConsulting.SharedKernel.Payments.PaymentStatuses;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Orders.CreateOrderFromBasket;

public record CreateOrderFromBasketCommand(Guid UserId, string Email) : IRequest<OperationDataResult<CreateOrderResult>>, ITransactionAddRequest, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [];
}

public class CreateOrderFromBasketHandler(IBasketRepository basketRepository, IBasketItemRepository basketItemRepository, IUserAddressRepository userAddressRepository, IOrderRepository orderRepository, IOrderItemRepository orderItemRepository, IEmailOutboxWriter outboxWriter, IEmailTemplate<OrderConfirmationEmailModel> confirmationTemplate, IPaymentGateway paymentGateway)
    : IRequestHandler<CreateOrderFromBasketCommand, OperationDataResult<CreateOrderResult>>
{
    public async Task<OperationDataResult<CreateOrderResult>> Handle(CreateOrderFromBasketCommand request, CancellationToken cancellationToken)
    {
        var basket = await basketRepository.GetAsync(b => b.UserId == request.UserId, cancellationToken: cancellationToken);
        if (basket is null)
        {
            return Result.BadRequest<CreateOrderResult>(BasketMessages.BasketNotFoundOrEmpty);
        }

        var basketItems = await basketItemRepository.GetListAsync(i => i.BasketId == basket.Id, size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);
        if (basketItems.Items.Count == 0)
        {
            return Result.BadRequest<CreateOrderResult>(BasketMessages.BasketNotFoundOrEmpty);
        }

        var shippingAddress = await userAddressRepository.GetAsync(a => a.UserId == request.UserId && a.IsShippingAddress,
            enableTracking: false, cancellationToken: cancellationToken);
        if (shippingAddress is null)
        {
            return Result.BadRequest<CreateOrderResult>(AddressMessages.ShippingAddressNotFound);
        }

        var billingAddress = await userAddressRepository.GetAsync(a => a.UserId == request.UserId && a.IsBillingAddress, enableTracking: false, cancellationToken: cancellationToken);

        if (billingAddress is null)
        {
            return Result.BadRequest<CreateOrderResult>(AddressMessages.BillingAddressNotFound);
        }

        var orderId = Guid.NewGuid();
        var total = basketItems.Items.Sum(i => TaxCalculator.Calculate(i.Price, i.Quantity, i.TaxRate).TotalPrice);

        // Payment is started before the Order row exists so OrderId can be used as the gateway's own
        // idempotency key and webhook reference - retrying this handler (e.g. after a timeout) can't
        // double-charge, and the eventual webhook already knows exactly which order to confirm.
        var paymentIntent = await paymentGateway.CreatePaymentIntentAsync(
            new CreatePaymentIntentRequest(total, "usd", orderId.ToString(), request.Email, IdempotencyKey: orderId.ToString()),
            cancellationToken);

        var order = await CreateOrderWithItemsAsync(orderId, request.UserId, shippingAddress.Id, billingAddress.Id, basketItems.Items, paymentIntent);

        var confirmationModel = new OrderConfirmationEmailModel(order.OrderNumber, basketItems.Items.Count, total);

        outboxWriter.Enqueue(request.Email, confirmationTemplate.Subject(confirmationModel), confirmationTemplate.Build(confirmationModel), sourceReference: $"Order:{order.Id}");

        foreach (var basketItem in basketItems.Items)
        {
            _ = await basketItemRepository.DeleteAsync(basketItem);
        }

        _ = await basketRepository.DeleteAsync(basket);

        return Result.Created(new CreateOrderResult(order.Id, paymentIntent.ClientSecret, order.OrderNumber), $"Order created: {order.OrderNumber}");
    }

    private async Task<Order> CreateOrderWithItemsAsync(Guid orderId, Guid userId, Guid shippingAddressId, Guid billingAddressId, IEnumerable<BasketItem> basketItems, PaymentIntentResult paymentIntent)
    {
        var order = new Order
        {
            Id = orderId,
            OrderNumber = OrderNumberGenerator.Generate(),
            OrderStatus = OrderStatuses.Pending,
            PaymentStatus = MapPaymentStatus(paymentIntent.Status),
            PaymentProvider = paymentGateway.ProviderName,
            ProviderPaymentId = paymentIntent.ProviderPaymentId,
            UserId = userId,
            ShippingAddressId = shippingAddressId,
            InvoiceAddressId = billingAddressId,
        };
        _ = await orderRepository.AddAsync(order);

        foreach (var basketItem in basketItems)
        {
            var (subTotalPrice, taxAmount, totalPrice) = TaxCalculator.Calculate(basketItem.Price, basketItem.Quantity, basketItem.TaxRate);
            _ = await orderItemRepository.AddAsync(new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                ServiceId = basketItem.ServiceId,
                Quantity = basketItem.Quantity,
                UnitPrice = basketItem.Price,
                TaxRate = basketItem.TaxRate,
                SubTotalPrice = subTotalPrice,
                TaxAmount = taxAmount,
                TotalPrice = totalPrice,
            });
        }

        return order;
    }

    /// <summary>The gateway's normalized status at intent-creation time - a synchronous "succeeded" (e.g. the Mock gateway, or a provider that captures immediately) marks the order Paid right away; anything else stays Pending until PaymentSucceededNotification/PaymentFailedNotification arrives from the webhook.</summary>
    private static string MapPaymentStatus(string gatewayStatus) => gatewayStatus == SharedPaymentStatuses.Succeeded ? OrderPaymentStatuses.Paid : OrderPaymentStatuses.Pending;
}
