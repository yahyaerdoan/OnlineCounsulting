using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Abstractions;
using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Rules;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Baskets.ClearBasket;

/// <summary>Empties the caller's basket.</summary>
public record ClearBasketCommand(Guid? UserId, Guid? GuestId) : IRequest<OperationResult>, ITransactionAddRequest;

public class ClearBasketHandler(IBasketRepository basketRepository) : IRequestHandler<ClearBasketCommand, OperationResult>
{
    public async Task<OperationResult> Handle(ClearBasketCommand request, CancellationToken cancellationToken)
    {
        var basket = await basketRepository.GetForOwnerAsync(request.UserId, request.GuestId, cancellationToken: cancellationToken);
        if (basket is null)
        {
            return BasketBusinessRules.BasketNotFound();
        }

        basket.Clear();
        _ = await basketRepository.UpdateAsync(basket);

        return Result.Success("Basket cleared successfully.");
    }
}
