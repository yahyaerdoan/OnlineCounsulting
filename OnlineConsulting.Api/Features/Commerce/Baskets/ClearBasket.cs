using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.ClearBasket;
using OnlineConsulting.SharedKernel.GuestIdentity;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Commerce.Baskets;

public class ClearBasket : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapDelete("/api/basket", Handle)
            .WithTags("Commerce/Baskets")
            .WithName("ClearBasket")
            .WithDescription("Removes every item from the current user's (or guest's) basket.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext, IGuestIdAccessor guestIdAccessor)
    {
        var (userId, guestId, error) = await BasketOwnerResolver.ResolveAsync(sender, httpContext, guestIdAccessor);
        if (error is not null)
        {
            return error;
        }

        var result = await sender.Send(new ClearBasketCommand(userId, guestId));
        return result.ToEnvelopedResult(httpContext);
    }
}
