using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Abstractions;
using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Constants;
using OnlineConsulting.Modules.Commerce.Domain;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using OnlineConsulting.SharedKernel.Catalog;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Baskets.AddBasketItem;

/// <summary>Deliberately not ISecureAddRequest since adding to a basket must work for anonymous guests, identified by GuestId instead of UserId.
/// Price and tax rate come from the catalog, never from the client - a client-supplied price would let anyone buy at any price.</summary>
public record AddBasketItemCommand(Guid? UserId, Guid? GuestId, Guid ServiceId, int Quantity)
    : IRequest<OperationResult>, ITransactionAddRequest;

/// <summary>Only Product services can be bought; Booking services go through the appointment flow instead.</summary>
public class AddBasketItemHandler(IBasketRepository basketRepository, IBasketItemRepository basketItemRepository, IServiceCatalogReader catalogReader) : IRequestHandler<AddBasketItemCommand, OperationResult>
{
    public async Task<OperationResult> Handle(AddBasketItemCommand request, CancellationToken cancellationToken)
    {
        var catalogEntry = await catalogReader.GetAsync(request.ServiceId, cancellationToken);
        if (catalogEntry is null)
        {
            return Result.NotFound(BasketMessages.ServiceNotFound);
        }

        if (catalogEntry.Kind != ServiceKinds.Product)
        {
            return Result.BadRequest(BasketMessages.ServiceIsBookedNotBought);
        }

        var basket = await BasketOwnerLookup.GetOrCreateAsync(basketRepository, request.UserId, request.GuestId, cancellationToken);

        var existingItem = await basketItemRepository.GetAsync(i => i.BasketId == basket.Id && i.ServiceId == request.ServiceId, cancellationToken: cancellationToken);
        if (existingItem is not null)
        {
            existingItem.Quantity += request.Quantity;
            existingItem.Price = catalogEntry.UnitPrice;
            existingItem.TaxRate = catalogEntry.TaxRate;
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
                Price = catalogEntry.UnitPrice,
                TaxRate = catalogEntry.TaxRate,
            };
            TaxCalculator.Apply(item);
            _ = await basketItemRepository.AddAsync(item);
        }

        await BasketTotalsCalculator.RecalculateAndSaveAsync(basket, basketItemRepository, basketRepository, cancellationToken);

        return Result.Created("Basket item added successfully.");
    }
}
