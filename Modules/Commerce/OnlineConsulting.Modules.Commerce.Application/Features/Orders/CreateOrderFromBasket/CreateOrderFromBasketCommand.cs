using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Addresses.Constants;
using OnlineConsulting.Modules.Commerce.Application.Features.Addresses.Contracts;
using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Constants;
using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Contracts;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Contracts;
using OnlineConsulting.Modules.Commerce.Domain;
using OnlineConsulting.SharedKernel.Authorization;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Orders.CreateOrderFromBasket;

// Checkout: converts the current user's basket into an order, snapshots their current shipping/
// billing address by id, then empties the basket. Crosses three groups (Baskets, Addresses,
// Orders) within this one module - that's fine, sibling groups in the same module share a
// DbContext/transaction boundary; only cross-MODULE references have to go through plain ids.
public record CreateOrderFromBasketCommand(Guid UserId) : IRequest<OperationDataResult<Guid>>, ITransactionAddRequest, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [GlobalOperationClaims.User];
}

public class CreateOrderFromBasketHandler(
    IBasketRepository basketRepository,
    IBasketItemRepository basketItemRepository,
    IUserAddressRepository userAddressRepository,
    IOrderRepository orderRepository,
    IOrderItemRepository orderItemRepository) : IRequestHandler<CreateOrderFromBasketCommand, OperationDataResult<Guid>>
{
    public async Task<OperationDataResult<Guid>> Handle(CreateOrderFromBasketCommand request, CancellationToken cancellationToken)
    {
        var basket = await basketRepository.GetAsync(b => b.UserId == request.UserId, cancellationToken: cancellationToken);
        if (basket is null)
            return Result.BadRequest<Guid>(BasketMessages.BasketNotFoundOrEmpty);

        var basketItems = await basketItemRepository.GetListAsync(i => i.BasketId == basket.Id, size: int.MaxValue, cancellationToken: cancellationToken);
        if (basketItems.Items.Count == 0)
            return Result.BadRequest<Guid>(BasketMessages.BasketNotFoundOrEmpty);

        var shippingAddress = await userAddressRepository.GetAsync(a => a.UserId == request.UserId && a.IsShippingAddress,
            enableTracking: false, cancellationToken: cancellationToken);
        if (shippingAddress is null)
            return Result.BadRequest<Guid>(AddressMessages.ShippingAddressNotFound);

        var billingAddress = await userAddressRepository.GetAsync(a => a.UserId == request.UserId && a.IsBillingAddress,
            enableTracking: false, cancellationToken: cancellationToken);
        if (billingAddress is null)
            return Result.BadRequest<Guid>(AddressMessages.BillingAddressNotFound);

        var order = await CreateOrderWithItemsAsync(request.UserId, shippingAddress.Id, billingAddress.Id, basketItems.Items);

        foreach (var basketItem in basketItems.Items)
            await basketItemRepository.DeleteAsync(basketItem);
        await basketRepository.DeleteAsync(basket);

        return Result.Created(order.Id, $"Order created: {order.OrderNumber}");
    }

    private async Task<Order> CreateOrderWithItemsAsync(Guid userId, Guid shippingAddressId, Guid billingAddressId, IEnumerable<BasketItem> basketItems)
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            OrderNumber = OrderNumberGenerator.Generate(),
            OrderStatus = OrderStatuses.Pending,
            PaymentStatus = PaymentStatuses.Paid,
            UserId = userId,
            ShippingAddressId = shippingAddressId,
            InvoiceAddressId = billingAddressId,
        };
        await orderRepository.AddAsync(order);

        foreach (var basketItem in basketItems)
        {
            var (subTotalPrice, taxAmount, totalPrice) = TaxCalculator.Calculate(basketItem.Price, basketItem.Quantity, basketItem.TaxRate);
            await orderItemRepository.AddAsync(new OrderItem
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
}
