using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.RemoveBasketItem;
using OnlineConsulting.SharedKernel.GuestIdentity;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Commerce.Baskets;

public class RemoveBasketItem : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapDelete("/api/basket/items/{id:guid}", Handle)
            .WithTags("Commerce/Baskets")
            .WithName("RemoveBasketItem")
            .WithDescription("Removes an item from the current user's (or guest's) basket.");
    }

    private static async Task<IResult> Handle(Guid id, ISender sender, HttpContext httpContext, IGuestIdAccessor guestIdAccessor)
    {
        var (userId, guestId, error) = await BasketOwnerResolver.ResolveAsync(sender, httpContext, guestIdAccessor);
        if (error is not null)
        {
            return error;
        }

        var result = await sender.Send(new RemoveBasketItemCommand(userId, guestId, id));
        return result.ToEnvelopedResult(httpContext);
    }
}
