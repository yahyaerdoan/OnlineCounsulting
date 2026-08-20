using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.GetBasketItemsCount;
using OnlineConsulting.SharedKernel.GuestIdentity;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Commerce.Baskets;

public class GetBasketItemsCount : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/basket/count", Handle)
            .WithTags("Commerce/Baskets")
            .WithName("GetBasketItemsCount")
            .WithDescription("Returns the number of items in the current user's (or guest's) basket.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext, IGuestIdAccessor guestIdAccessor)
    {
        var (userId, guestId, error) = await BasketOwnerResolver.ResolveAsync(sender, httpContext, guestIdAccessor);
        if (error is not null)
        {
            return error;
        }

        var result = await sender.Send(new GetBasketItemsCountQuery(userId, guestId));
        return result.ToEnvelopedResult(httpContext);
    }
}
