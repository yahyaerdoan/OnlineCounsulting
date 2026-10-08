using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Abstractions;
using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Rules;
using OnlineConsulting.SharedKernel.Transactions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Baskets.RemoveBasketItem;

/// <summary>Removes a line from the caller's basket; works for guests too.</summary>
public record RemoveBasketItemCommand(Guid? UserId, Guid? GuestId, Guid BasketItemId) : IRequest<OperationResult>, ICommerceTransactionRequest;

public class RemoveBasketItemHandler(IBasketRepository basketRepository) : IRequestHandler<RemoveBasketItemCommand, OperationResult>
{
    public async Task<OperationResult> Handle(RemoveBasketItemCommand request, CancellationToken cancellationToken)
    {
        var basket = await basketRepository.GetForOwnerAsync(request.UserId, request.GuestId, cancellationToken: cancellationToken);
        if (basket is null)
        {
            return BasketBusinessRules.BasketNotFound();
        }

        if (basket.FindItem(request.BasketItemId) is null)
        {
            return BasketBusinessRules.BasketItemNotFound(request.BasketItemId);
        }

        basket.RemoveItem(request.BasketItemId);
        _ = await basketRepository.UpdateAsync(basket, cancellationToken: cancellationToken);

        return Result.Success("Basket item removed successfully.");
    }
}
