using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Common;
using OnlineConsulting.Modules.Commerce.Application.Common.Templates;
using OnlineConsulting.Modules.Commerce.Application.Features.Addresses.Constants;
using OnlineConsulting.Modules.Commerce.Application.Features.Addresses.Contracts;
using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Constants;
using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Contracts;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Contracts;
using OnlineConsulting.Modules.Commerce.Domain;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.Notifications;
using OnlineConsulting.SharedKernel.Notifications.Templates;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Orders.CreateOrderFromBasket;

public record CreateOrderFromBasketCommand(Guid UserId, string Email) : IRequest<OperationDataResult<Guid>>, ITransactionAddRequest, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [GlobalOperationClaims.User];
}

public class CreateOrderFromBasketHandler(IBasketRepository basketRepository, IBasketItemRepository basketItemRepository, IUserAddressRepository userAddressRepository, IOrderRepository orderRepository, IOrderItemRepository orderItemRepository, IEmailOutboxWriter outboxWriter, IEmailTemplate<OrderConfirmationEmailModel> confirmationTemplate)
    : IRequestHandler<CreateOrderFromBasketCommand, OperationDataResult<Guid>>
{
    public async Task<OperationDataResult<Guid>> Handle(CreateOrderFromBasketCommand request, CancellationToken cancellationToken)
    {
        var basket = await basketRepository.GetAsync(b => b.UserId == request.UserId, cancellationToken: cancellationToken);
        if (basket is null)
            return Result.BadRequest<Guid>(BasketMessages.BasketNotFoundOrEmpty);

        var basketItems = await basketItemRepository.GetListAsync(i => i.BasketId == basket.Id, size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);
        if (basketItems.Items.Count == 0)
            return Result.BadRequest<Guid>(BasketMessages.BasketNotFoundOrEmpty);

        var shippingAddress = await userAddressRepository.GetAsync(a => a.UserId == request.UserId && a.IsShippingAddress,
            enableTracking: false, cancellationToken: cancellationToken);
        if (shippingAddress is null)
            return Result.BadRequest<Guid>(AddressMessages.ShippingAddressNotFound);

        var billingAddress = await userAddressRepository.GetAsync(a => a.UserId == request.UserId && a.IsBillingAddress, enableTracking: false, cancellationToken: cancellationToken);

        if (billingAddress is null)
            return Result.BadRequest<Guid>(AddressMessages.BillingAddressNotFound);

        var (order, total) = await CreateOrderWithItemsAsync(request.UserId, shippingAddress.Id, billingAddress.Id, basketItems.Items);

        var confirmationModel = new OrderConfirmationEmailModel(order.OrderNumber, basketItems.Items.Count, total);

        outboxWriter.Enqueue(request.Email, confirmationTemplate.Subject(confirmationModel), confirmationTemplate.Build(confirmationModel), sourceReference: $"Order:{order.Id}");

        foreach (var basketItem in basketItems.Items)
            await basketItemRepository.DeleteAsync(basketItem);

        await basketRepository.DeleteAsync(basket);

        return Result.Created(order.Id, $"Order created: {order.OrderNumber}");
    }

    private async Task<(Order Order, decimal Total)> CreateOrderWithItemsAsync(Guid userId, Guid shippingAddressId, Guid billingAddressId, IEnumerable<BasketItem> basketItems)
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

        var total = 0m;
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
            total += totalPrice;
        }

        return (order, total);
    }
}
