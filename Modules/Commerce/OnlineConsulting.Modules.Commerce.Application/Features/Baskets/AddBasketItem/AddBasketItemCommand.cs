using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Contracts;
using OnlineConsulting.Modules.Commerce.Domain;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Baskets.AddBasketItem;

// Price/TaxRate are supplied by the caller rather than looked up from the catalog - the legacy
// Service entity hasn't been migrated into a module yet, and Commerce doesn't take a dependency on
// the old shared DbContext. Once a Catalog module exists, this should revalidate price server-side
// against it instead of trusting the caller.
// Deliberately not ISecureAddRequest - adding to a basket must work for anonymous guests too
// (UserId is null, GuestId carries the cookie-issued id instead). Checkout is what requires login.
public record AddBasketItemCommand(Guid? UserId, Guid? GuestId, Guid ServiceId, int Quantity, decimal Price, int TaxRate)
    : IRequest<OperationResult>, ITransactionAddRequest;

public class AddBasketItemHandler(IBasketRepository basketRepository, IBasketItemRepository basketItemRepository)
    : IRequestHandler<AddBasketItemCommand, OperationResult>
{
    public async Task<OperationResult> Handle(AddBasketItemCommand request, CancellationToken cancellationToken)
    {
        var basket = await basketRepository.GetAsync(BasketOwnerLookup.Predicate(request.UserId, request.GuestId), cancellationToken: cancellationToken);
        if (basket is null)
        {
            basket = new Basket { Id = Guid.NewGuid(), UserId = request.UserId, GuestId = request.GuestId };
            await basketRepository.AddAsync(basket);
        }

        var existingItem = await basketItemRepository.GetAsync(i => i.BasketId == basket.Id && i.ServiceId == request.ServiceId, cancellationToken: cancellationToken);
        if (existingItem is not null)
        {
            existingItem.Quantity = request.Quantity;
            (existingItem.SubTotalPrice, existingItem.TaxAmount, existingItem.TotalPrice) =
                TaxCalculator.Calculate(existingItem.Price, existingItem.Quantity, existingItem.TaxRate);
            await basketItemRepository.UpdateAsync(existingItem);
        }
        else
        {
            var (subTotalPrice, taxAmount, totalPrice) = TaxCalculator.Calculate(request.Price, request.Quantity, request.TaxRate);
            var item = new BasketItem
            {
                Id = Guid.NewGuid(),
                BasketId = basket.Id,
                ServiceId = request.ServiceId,
                Quantity = request.Quantity,
                Price = request.Price,
                TaxRate = request.TaxRate,
                SubTotalPrice = subTotalPrice,
                TaxAmount = taxAmount,
                TotalPrice = totalPrice,
            };
            await basketItemRepository.AddAsync(item);
        }

        await UpdateBasketTotalsAsync(basket, cancellationToken);

        return Result.Created("Basket item added successfully.");
    }

    private async Task UpdateBasketTotalsAsync(Basket basket, CancellationToken cancellationToken)
    {
        var items = await basketItemRepository.GetListAsync(i => i.BasketId == basket.Id, size: int.MaxValue, cancellationToken: cancellationToken);
        (basket.Quantity, basket.SubTotalPrice, basket.TotalPrice) = BasketTotalsCalculator.Calculate(items.Items);
        await basketRepository.UpdateAsync(basket);
    }
}
