using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Abstractions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Baskets.GetBasketItemsCount;

/// <summary>Number of lines in the caller's basket (the cart badge); 0 when there is no basket yet.</summary>
public record GetBasketItemsCountQuery(Guid? UserId, Guid? GuestId) : IRequest<OperationDataResult<int>>;

public class GetBasketItemsCountHandler(IBasketRepository basketRepository) : IRequestHandler<GetBasketItemsCountQuery, OperationDataResult<int>>
{
    public async Task<OperationDataResult<int>> Handle(GetBasketItemsCountQuery request, CancellationToken cancellationToken)
    {
        var basket = await basketRepository.GetForOwnerAsync(request.UserId, request.GuestId, enableTracking: false, cancellationToken);

        return basket is null
            ? Result.Success(0, "No basket yet.")
            : Result.Success(basket.Items.Count, "Basket item count retrieved successfully.");
    }
}
