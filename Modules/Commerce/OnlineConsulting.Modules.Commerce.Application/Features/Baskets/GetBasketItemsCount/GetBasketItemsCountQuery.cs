using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Contracts;
using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Abstractions;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Baskets.GetBasketItemsCount;

public record GetBasketItemsCountQuery(Guid? UserId, Guid? GuestId) : IRequest<OperationDataResult<int>>;

public class GetBasketItemsCountHandler(IBasketRepository basketRepository, IBasketItemRepository basketItemRepository)
    : IRequestHandler<GetBasketItemsCountQuery, OperationDataResult<int>>
{
    public async Task<OperationDataResult<int>> Handle(GetBasketItemsCountQuery request, CancellationToken cancellationToken)
    {
        var basket = await basketRepository.GetAsync(BasketOwnerLookup.Predicate(request.UserId, request.GuestId),
            enableTracking: false, cancellationToken: cancellationToken);
        if (basket is null)
            return Result.Success(0, "No basket yet.");

        var items = await basketItemRepository.GetListAsync(i => i.BasketId == basket.Id, size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);

        return Result.Success(items.Count, "Basket item count retrieved successfully.");
    }
}
