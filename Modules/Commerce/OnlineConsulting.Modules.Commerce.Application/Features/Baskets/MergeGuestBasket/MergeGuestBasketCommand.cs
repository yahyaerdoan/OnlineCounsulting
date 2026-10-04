using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Abstractions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Baskets.MergeGuestBasket;

/// <summary>Moves a guest basket into the user's basket after sign-in; matching lines add up. Runs before Commerce sees the new token, so it is not secured.</summary>
public record MergeGuestBasketCommand(Guid UserId, Guid GuestId) : IRequest<OperationResult>, ITransactionAddRequest;

public class MergeGuestBasketHandler(IBasketRepository basketRepository) : IRequestHandler<MergeGuestBasketCommand, OperationResult>
{
    public async Task<OperationResult> Handle(MergeGuestBasketCommand request, CancellationToken cancellationToken)
    {
        var guestBasket = await basketRepository.GetForOwnerAsync(null, request.GuestId, cancellationToken: cancellationToken);
        if (guestBasket is null)
        {
            return Result.Success("No guest basket to merge.");
        }

        var userBasket = await BasketOwnerLookup.GetOrOpenAsync(basketRepository, request.UserId, null, cancellationToken);
        userBasket.MergeFrom(guestBasket);

        _ = await basketRepository.UpdateAsync(userBasket, cancellationToken: cancellationToken);
        _ = await basketRepository.DeleteAsync(guestBasket, cancellationToken: cancellationToken);

        return Result.Success("Guest basket merged successfully.");
    }
}
