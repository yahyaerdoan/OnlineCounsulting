using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Constants;
using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Contracts;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Baskets.GetBasket;

// UserId is set for a logged-in caller, GuestId for an anonymous one - exactly one is non-null,
// resolved at the Api layer (see BasketOwnerResolver).
public record GetBasketQuery(Guid? UserId, Guid? GuestId) : IRequest<OperationDataResult<BasketResponse>>;

public class GetBasketHandler(IBasketRepository basketRepository, IBasketItemRepository basketItemRepository)
    : IRequestHandler<GetBasketQuery, OperationDataResult<BasketResponse>>
{
    public async Task<OperationDataResult<BasketResponse>> Handle(GetBasketQuery request, CancellationToken cancellationToken)
    {
        // Read-only lookup - no mutation follows, so track nothing.
        var basket = await basketRepository.GetAsync(BasketOwnerLookup.Predicate(request.UserId, request.GuestId),
            enableTracking: false, cancellationToken: cancellationToken);
        if (basket is null)
            return Result.NotFound<BasketResponse>(BasketMessages.BasketNotFound);

        var items = await basketItemRepository.GetListAsync(i => i.BasketId == basket.Id, size: int.MaxValue, cancellationToken: cancellationToken);

        return Result.Success(BasketResponse.FromDomain(basket, items.Items), "Basket retrieved successfully.");
    }
}
