using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Abstractions;
using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Rules;
using OnlineConsulting.SharedKernel.Transactions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Baskets.SetBasketItemQuantity;

/// <summary>Sets a line's quantity to an absolute value (the cart's +/- stepper); works for guests too.</summary>
public record SetBasketItemQuantityCommand(Guid? UserId, Guid? GuestId, Guid BasketItemId, int Quantity) : IRequest<OperationResult>, ICommerceTransactionRequest;

public class SetBasketItemQuantityHandler(IBasketRepository basketRepository) : IRequestHandler<SetBasketItemQuantityCommand, OperationResult>
{
    public async Task<OperationResult> Handle(SetBasketItemQuantityCommand request, CancellationToken cancellationToken)
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

        basket.SetItemQuantity(request.BasketItemId, request.Quantity);
        _ = await basketRepository.UpdateAsync(basket, cancellationToken: cancellationToken);

        return Result.Success("Basket item quantity updated successfully.");
    }
}
