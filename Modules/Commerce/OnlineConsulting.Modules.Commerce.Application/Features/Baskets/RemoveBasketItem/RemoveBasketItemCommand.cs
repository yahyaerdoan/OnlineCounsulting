using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Abstractions;
using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Rules;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Baskets.RemoveBasketItem;

/// <summary>Not ISecureAddRequest - see AddBasketItemCommand.</summary>
public record RemoveBasketItemCommand(Guid? UserId, Guid? GuestId, Guid BasketItemId) : IRequest<OperationResult>, ITransactionAddRequest;

public class RemoveBasketItemHandler(IBasketRepository basketRepository, IBasketItemRepository basketItemRepository)
    : IRequestHandler<RemoveBasketItemCommand, OperationResult>
{
    public async Task<OperationResult> Handle(RemoveBasketItemCommand request, CancellationToken cancellationToken)
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

        _ = await basketItemRepository.DeleteAsync(item);

        await BasketTotalsCalculator.RecalculateAndSaveAsync(basket, basketItemRepository, basketRepository, cancellationToken);

        return Result.Success("Basket item removed successfully.");
    }
}
