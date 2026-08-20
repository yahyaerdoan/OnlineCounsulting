using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.GetBasket;
using OnlineConsulting.SharedKernel.GuestIdentity;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Commerce.Baskets;

public class GetBasket : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/basket", Handle)
            .WithTags("Commerce/Baskets")
            .WithName("GetBasket")
            .WithDescription("Returns the current user's (or guest's) basket, with its items.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext, IGuestIdAccessor guestIdAccessor)
    {
        var (userId, guestId, error) = await BasketOwnerResolver.ResolveAsync(sender, httpContext, guestIdAccessor);
        if (error is not null)
        {
            return error;
        }

        var result = await sender.Send(new GetBasketQuery(userId, guestId));
        return result.ToEnvelopedResult(httpContext);
    }
}
