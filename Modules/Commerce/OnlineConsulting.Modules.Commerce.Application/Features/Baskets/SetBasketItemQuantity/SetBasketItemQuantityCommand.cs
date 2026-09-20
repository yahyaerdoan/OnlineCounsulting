using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Abstractions;
using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Rules;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Baskets.SetBasketItemQuantity;

/// <summary>Sets a line's quantity to an absolute value - the cart page's +/- stepper.
/// Not ISecureAddRequest - see AddBasketItemCommand.</summary>
public record SetBasketItemQuantityCommand(Guid? UserId, Guid? GuestId, Guid BasketItemId, int Quantity) : IRequest<OperationResult>, ITransactionAddRequest;

public class SetBasketItemQuantityHandler(IBasketRepository basketRepository, IBasketItemRepository basketItemRepository)
    : IRequestHandler<SetBasketItemQuantityCommand, OperationResult>
{
    public async Task<OperationResult> Handle(SetBasketItemQuantityCommand request, CancellationToken cancellationToken)
    {
        var basket = await basketRepository.GetAsync(BasketOwnerLookup.Predicate(request.UserId, request.GuestId), cancellationToken: cancellationToken);
        if (basket is null)
        {
            return BasketBusinessRules.BasketNotFound();
        }

        var item = await basketItemRepository.GetAsync(i => i.Id == request.BasketItemId && i.BasketId == basket.Id, cancellationToken: cancellationToken);
        if (item is null)
        {
            return BasketBusinessRules.BasketItemNotFound(request.BasketItemId);
        }

        item.Quantity = request.Quantity;
        TaxCalculator.Apply(item);
        _ = await basketItemRepository.UpdateAsync(item);

        await BasketTotalsCalculator.RecalculateAndSaveAsync(basket, basketItemRepository, basketRepository, cancellationToken);

        return Result.Success("Basket item quantity updated successfully.");
    }
}
