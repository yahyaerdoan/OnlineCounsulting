using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Abstractions;
using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Constants;
using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Contracts;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Baskets.GetBasket;

/// <summary>The caller's basket; exactly one of UserId and GuestId is set (resolved by the Api's BasketOwnerResolver).</summary>
public record GetBasketQuery(Guid? UserId, Guid? GuestId) : IRequest<OperationDataResult<BasketResponse>>;

public class GetBasketHandler(IBasketRepository basketRepository) : IRequestHandler<GetBasketQuery, OperationDataResult<BasketResponse>>
{
    public async Task<OperationDataResult<BasketResponse>> Handle(GetBasketQuery request, CancellationToken cancellationToken)
    {
        var basket = await basketRepository.GetForOwnerAsync(request.UserId, request.GuestId, enableTracking: false, cancellationToken);

        return basket is null
            ? Result.NotFound<BasketResponse>(BasketMessages.BasketNotFound)
            : Result.Success(BasketResponse.FromDomain(basket), "Basket retrieved successfully.");
    }
}
