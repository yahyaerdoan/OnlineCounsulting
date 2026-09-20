using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Abstractions;
using OnlineConsulting.Modules.Commerce.Domain;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Baskets.AddBasketItem;

/// <summary>Deliberately not ISecureAddRequest since adding to a basket must work for anonymous guests, identified by GuestId instead of UserId.</summary>
public record AddBasketItemCommand(Guid? UserId, Guid? GuestId, Guid ServiceId, int Quantity, decimal Price, int TaxRate)
    : IRequest<OperationResult>, ITransactionAddRequest;

public class AddBasketItemHandler(IBasketRepository basketRepository, IBasketItemRepository basketItemRepository)
    : IRequestHandler<AddBasketItemCommand, OperationResult>
{
    public async Task<OperationResult> Handle(AddBasketItemCommand request, CancellationToken cancellationToken)
    {
        var basket = await BasketOwnerLookup.GetOrCreateAsync(basketRepository, request.UserId, request.GuestId, cancellationToken);

        var existingItem = await basketItemRepository.GetAsync(i => i.BasketId == basket.Id && i.ServiceId == request.ServiceId, cancellationToken: cancellationToken);
        if (existingItem is not null)
        {
            existingItem.Quantity += request.Quantity;
            TaxCalculator.Apply(existingItem);
            _ = await basketItemRepository.UpdateAsync(existingItem);
        }
        else
        {
            var item = new BasketItem
            {
                BasketId = basket.Id,
                ServiceId = request.ServiceId,
                Quantity = request.Quantity,
                Price = request.Price,
                TaxRate = request.TaxRate,
            };
            TaxCalculator.Apply(item);
            _ = await basketItemRepository.AddAsync(item);
        }

        await BasketTotalsCalculator.RecalculateAndSaveAsync(basket, basketItemRepository, basketRepository, cancellationToken);

        return Result.Created("Basket item added successfully.");
    }
}
