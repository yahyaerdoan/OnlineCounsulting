using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Contracts;
using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Abstractions;
using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Rules;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Baskets.ClearBasket;

/// <summary>Used both as a direct "empty my cart" endpoint and internally by checkout once a basket has been converted into an order.</summary>
public record ClearBasketCommand(Guid? UserId, Guid? GuestId) : IRequest<OperationResult>, ITransactionAddRequest;

public class ClearBasketHandler(IBasketRepository basketRepository, IBasketItemRepository basketItemRepository)
    : IRequestHandler<ClearBasketCommand, OperationResult>
{
    public async Task<OperationResult> Handle(ClearBasketCommand request, CancellationToken cancellationToken)
    {
        var basket = await basketRepository.GetAsync(BasketOwnerLookup.Predicate(request.UserId, request.GuestId), cancellationToken: cancellationToken);
        if (basket is null)
            return BasketBusinessRules.BasketNotFound();

        var items = await basketItemRepository.GetListAsync(i => i.BasketId == basket.Id, size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);
        foreach (var item in items.Items)
            await basketItemRepository.DeleteAsync(item);

        (basket.Quantity, basket.SubTotalPrice, basket.TotalPrice) = BasketTotalsCalculator.Calculate([]);
        await basketRepository.UpdateAsync(basket);

        return Result.Success("Basket cleared successfully.");
    }
}
