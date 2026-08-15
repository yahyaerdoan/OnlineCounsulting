using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Contracts;
using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Rules;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Baskets.RemoveBasketItem;

// Not ISecureAddRequest - see AddBasketItemCommand.
public record RemoveBasketItemCommand(Guid? UserId, Guid? GuestId, Guid BasketItemId) : IRequest<OperationResult>, ITransactionAddRequest;

public class RemoveBasketItemHandler(IBasketRepository basketRepository, IBasketItemRepository basketItemRepository)
    : IRequestHandler<RemoveBasketItemCommand, OperationResult>
{
    public async Task<OperationResult> Handle(RemoveBasketItemCommand request, CancellationToken cancellationToken)
    {
        var basket = await basketRepository.GetAsync(BasketOwnerLookup.Predicate(request.UserId, request.GuestId), cancellationToken: cancellationToken);
        if (basket is null)
            return BasketBusinessRules.BasketNotFound();

        var item = await basketItemRepository.GetAsync(i => i.Id == request.BasketItemId && i.BasketId == basket.Id, cancellationToken: cancellationToken);
        if (item is null)
            return BasketBusinessRules.BasketItemNotFound(request.BasketItemId);

        await basketItemRepository.DeleteAsync(item);

        var remainingItems = await basketItemRepository.GetListAsync(i => i.BasketId == basket.Id, size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);
        (basket.Quantity, basket.SubTotalPrice, basket.TotalPrice) = BasketTotalsCalculator.Calculate(remainingItems.Items);
        await basketRepository.UpdateAsync(basket);

        return Result.Success("Basket item removed successfully.");
    }
}
