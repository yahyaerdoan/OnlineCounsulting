using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Addresses.Abstractions;
using OnlineConsulting.Modules.Commerce.Application.Features.Addresses.Constants;
using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Abstractions;
using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Constants;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Abstractions;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Contracts;
using OnlineConsulting.Modules.Commerce.Domain;
using OnlineConsulting.SharedKernel.Payments;
using OnlineConsulting.SharedKernel.Tenancy;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;
using SharedPaymentStatuses = OnlineConsulting.SharedKernel.Payments.PaymentStatuses;
using OnlineConsulting.SharedKernel.Catalog;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Orders.CreateOrderFromBasket;

/// <summary>Turns the caller's basket into an order at current catalog prices and starts its payment.</summary>
public record CreateOrderFromBasketCommand(Guid UserId, string Email) : IRequest<OperationDataResult<CreateOrderResult>>, ITransactionAddRequest, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [];
}

public class CreateOrderFromBasketHandler(IBasketRepository basketRepository,
                                          IUserAddressRepository userAddressRepository,
                                          IOrderRepository orderRepository,
                                          IOrderItemRepository orderItemRepository,
                                          IPaymentGateway paymentGateway,
                                          IServiceCatalogReader catalogReader,
                                          IOrderFulfillment fulfillment)
    : IRequestHandler<CreateOrderFromBasketCommand, OperationDataResult<CreateOrderResult>>
{
    public async Task<OperationDataResult<CreateOrderResult>> Handle(CreateOrderFromBasketCommand request, CancellationToken cancellationToken)
    {
        var basket = await basketRepository.GetForOwnerAsync(request.UserId, null, enableTracking: false, cancellationToken);

        if (basket is null)
        {
            return Result.NotFound<CreateOrderResult>(BasketMessages.BasketNotFoundOrEmpty);
        }

        if (basket.IsEmpty)
        {
            return Result.Conflict<CreateOrderResult>(BasketMessages.BasketNotFoundOrEmpty);
        }

        var catalog = await catalogReader.GetManyAsync(basket.Items.Select(i => i.ServiceId), cancellationToken);

        foreach (var basketItem in basket.Items)
        {
            if (!catalog.TryGetValue(basketItem.ServiceId, out var entry) || entry.Kind != ServiceKinds.Product)
            {
                return Result.Conflict<CreateOrderResult>(BasketMessages.ItemNoLongerPurchasable);
            }

            basket.Reprice(basketItem.ServiceId, entry.UnitPrice, entry.TaxRate);
        }

        var shippingAddress = await userAddressRepository.GetAsync(a => a.UserId == request.UserId && a.IsShippingAddress, enableTracking: false, cancellationToken: cancellationToken);

        if (shippingAddress is null)
        {
            return Result.Conflict<CreateOrderResult>(AddressMessages.ShippingAddressNotFound);
        }

        var billingAddress = await userAddressRepository.GetAsync(a => a.UserId == request.UserId && a.IsBillingAddress, enableTracking: false, cancellationToken: cancellationToken);

        if (billingAddress is null)
        {
            return Result.Conflict<CreateOrderResult>(AddressMessages.BillingAddressNotFound);
        }

        var orderId = SequentialGuidTenantEntity.NewId();
        var total = basket.TotalPrice;

        var (failure, paymentIntent) = await PaymentGatewayCall.RunWithResultAsync(() =>
        paymentGateway.CreatePaymentIntentAsync(new CreatePaymentIntentRequest(total, "usd", orderId.ToString(), request.Email, IdempotencyKey: orderId.ToString()), cancellationToken), "Could not start payment for your order. Please try again.");

        if (failure is not null || paymentIntent is null)
        {
            return Result.BadGateway<CreateOrderResult>(failure?.Detail ?? "Could not start payment for your order.");
        }

        var (order, _) = await CreateOrderWithItemsAsync(orderId, request.UserId, shippingAddress.Id, billingAddress.Id, basket.Items, paymentIntent, cancellationToken);

        if (order.PaymentStatus == OrderPaymentStatuses.Paid)
        {
            await fulfillment.CompletePaidAsync(order, cancellationToken);
        }

        var clientSecretForClient = paymentIntent.Status == SharedPaymentStatuses.Succeeded ? null : paymentIntent.ClientSecret;

        return Result.Created(new CreateOrderResult(order.Id, clientSecretForClient, order.OrderNumber), $"Order created: {order.OrderNumber}");
    }

    private async Task<(Order Order, List<OrderItem> Items)> CreateOrderWithItemsAsync(Guid orderId, Guid userId, Guid shippingAddressId, Guid billingAddressId, IEnumerable<BasketItem> basketItems, PaymentIntentResult paymentIntent,
        CancellationToken cancellationToken)
    {
        var order = Order.Place(orderId, OrderNumberGenerator.Generate(),
            userId, shippingAddressId, billingAddressId, paymentGateway.ProviderName, paymentIntent.ProviderPaymentId, paidAtCheckout: paymentIntent.Status == SharedPaymentStatuses.Succeeded);

        _ = await orderRepository.AddAsync(order, cancellationToken: cancellationToken);

        List<OrderItem> orderItems = [];

        foreach (var basketItem in basketItems)
        {
            var orderItem = new OrderItem
            {
                OrderId = order.Id,
                ServiceId = basketItem.ServiceId,
                Quantity = basketItem.Quantity,
                UnitPrice = basketItem.Price,
                TaxRate = basketItem.TaxRate,
            };

            TaxCalculator.Apply(orderItem);

            _ = await orderItemRepository.AddAsync(orderItem, cancellationToken: cancellationToken);

            orderItems.Add(orderItem);
        }

        return (order, orderItems);
    }
}
