using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.GetBasket;
using OnlineConsulting.SharedKernel.CurrentUser;
using OnlineConsulting.SharedKernel.GuestIdentity;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Commerce.Baskets;

public class GetBasket : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/basket", Handle)
            .WithTags("Commerce/Baskets")
            .WithName("GetBasket")
            .WithDescription("Returns the current user's (or guest's) basket, with its items.");
    }

    private static async Task<IResult> Handle(ICurrentUserAccessor currentUser, ISender sender, HttpContext httpContext, IGuestIdAccessor guestIdAccessor)
    {
        var (userId, guestId) = BasketOwnerResolver.Resolve(currentUser, guestIdAccessor);

        var result = await sender.Send(new GetBasketQuery(userId, guestId));
        return result.ToEnvelopedResult(httpContext);
    }
}
