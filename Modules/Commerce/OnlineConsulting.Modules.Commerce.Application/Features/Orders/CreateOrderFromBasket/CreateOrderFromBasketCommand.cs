using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Addresses.Abstractions;
using OnlineConsulting.Modules.Commerce.Application.Features.Addresses.Constants;
using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Abstractions;
using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Constants;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Abstractions;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Constants;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Contracts;
using OnlineConsulting.Modules.Commerce.Domain;
using OnlineConsulting.SharedKernel.Payments;
using OnlineConsulting.SharedKernel.Persistence;
using OnlineConsulting.SharedKernel.Tenancy;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;
using OrderPaymentStatuses = OnlineConsulting.Modules.Commerce.Application.Features.Orders.Constants.PaymentStatuses;
using SharedPaymentStatuses = OnlineConsulting.SharedKernel.Payments.PaymentStatuses;
using OnlineConsulting.SharedKernel.Catalog;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Orders.CreateOrderFromBasket;

/// <summary>Converts the user's basket into an order and starts payment.</summary>
public record CreateOrderFromBasketCommand(Guid UserId, string Email) : IRequest<OperationDataResult<CreateOrderResult>>, ITransactionAddRequest, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [];
}

/// <summary>
/// Creates an order and payment intent from the user's basket. The order id also serves as the gateway
/// idempotency key so retries cannot double-charge. A synchronous-Paid result sends the confirmation email
/// and clears the basket immediately; an async/Pending charge leaves both for <c>OnPaymentStatusChangedHandler</c>
/// to finish once the payment webhook confirms it. Every line is re-priced from the catalog at checkout (the price at payment
/// time is what's charged), and lines that are no longer purchasable Products stop the checkout.
/// </summary>
public class CreateOrderFromBasketHandler(IBasketRepository basketRepository, IBasketItemRepository basketItemRepository, IUserAddressRepository userAddressRepository, IOrderRepository orderRepository, IOrderItemRepository orderItemRepository, IPaymentGateway paymentGateway, IServiceCatalogReader catalogReader, IOrderFulfillment fulfillment)
    : IRequestHandler<CreateOrderFromBasketCommand, OperationDataResult<CreateOrderResult>>
{
    public async Task<OperationDataResult<CreateOrderResult>> Handle(CreateOrderFromBasketCommand request, CancellationToken cancellationToken)
    {
        var basket = await basketRepository.GetAsync(b => b.UserId == request.UserId, cancellationToken: cancellationToken);

        if (basket is null)
        {
            return Result.NotFound<CreateOrderResult>(BasketMessages.BasketNotFoundOrEmpty);
        }

        var basketItems = await basketItemRepository.GetListAsync(i => i.BasketId == basket.Id, orderBy: q => q.OrderBy(i => i.Id), size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);

        if (basketItems.Items.Count == 0)
        {
            return Result.Conflict<CreateOrderResult>(BasketMessages.BasketNotFoundOrEmpty);
        }

        var catalog = await catalogReader.GetManyAsync(basketItems.Items.Select(i => i.ServiceId), cancellationToken);
        foreach (var basketItem in basketItems.Items)
        {
            if (!catalog.TryGetValue(basketItem.ServiceId, out var entry) || entry.Kind != ServiceKinds.Product)
            {
                return Result.Conflict<CreateOrderResult>(BasketMessages.ItemNoLongerPurchasable);
            }

            basketItem.Price = entry.UnitPrice;
            basketItem.TaxRate = entry.TaxRate;
            TaxCalculator.Apply(basketItem);
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
        var total = basketItems.Items.Sum(i => TaxCalculator.Calculate(i.Price, i.Quantity, i.TaxRate).TotalPrice);

        var (failure, paymentIntent) = await PaymentGatewayCall.RunWithResultAsync(() =>
        paymentGateway.CreatePaymentIntentAsync(new CreatePaymentIntentRequest(total, "usd", orderId.ToString(), request.Email, IdempotencyKey: orderId.ToString()), cancellationToken), "Could not start payment for your order. Please try again.");

        if (failure is not null || paymentIntent is null)
        {
            return Result.BadGateway<CreateOrderResult>(failure?.Detail ?? "Could not start payment for your order.");
        }

        var (order, _) = await CreateOrderWithItemsAsync(orderId, request.UserId, shippingAddress.Id, billingAddress.Id, basketItems.Items, paymentIntent);

        if (order.PaymentStatus == OrderPaymentStatuses.Paid)
        {
            await fulfillment.CompletePaidAsync(order, cancellationToken);
        }

        var clientSecretForClient = paymentIntent.Status == SharedPaymentStatuses.Succeeded ? null : paymentIntent.ClientSecret;

        return Result.Created(new CreateOrderResult(order.Id, clientSecretForClient, order.OrderNumber), $"Order created: {order.OrderNumber}");
    }

    private async Task<(Order Order, List<OrderItem> Items)> CreateOrderWithItemsAsync(Guid orderId, Guid userId, Guid shippingAddressId, Guid billingAddressId, IEnumerable<BasketItem> basketItems, PaymentIntentResult paymentIntent)
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

            _ = await orderItemRepository.AddAsync(orderItem);
            orderItems.Add(orderItem);
        }

        return (order, orderItems);
    }

    /// <summary>Synchronous "succeeded" marks the order Paid immediately; otherwise stays Pending until the webhook notification arrives.</summary>
    private static string MapPaymentStatus(string gatewayStatus) => gatewayStatus == SharedPaymentStatuses.Succeeded ? OrderPaymentStatuses.Paid : OrderPaymentStatuses.Pending;
}
