using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.ClearBasket;
using OnlineConsulting.SharedKernel.CurrentUser;
using OnlineConsulting.SharedKernel.GuestIdentity;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Commerce.Baskets;

public class ClearBasket : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapDelete("/basket", Handle)
            .WithTags("Commerce/Baskets")
            .WithName("ClearBasket")
            .WithDescription("Removes every item from the current user's (or guest's) basket.");
    }

    private static async Task<IResult> Handle(ICurrentUserAccessor currentUser, ISender sender, HttpContext httpContext, IGuestIdAccessor guestIdAccessor)
    {
        var (userId, guestId) = BasketOwnerResolver.Resolve(currentUser, guestIdAccessor);

        var result = await sender.Send(new ClearBasketCommand(userId, guestId));
        return result.ToEnvelopedResult(httpContext);
    }
}
