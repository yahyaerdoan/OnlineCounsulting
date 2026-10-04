using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.SetBasketItemQuantity;
using OnlineConsulting.SharedKernel.CurrentUser;
using OnlineConsulting.SharedKernel.GuestIdentity;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Commerce.Baskets;

public class SetBasketItemQuantity : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/basket/items/{id:guid}", Handle)
            .WithTags("Commerce/Baskets")
            .WithName("SetBasketItemQuantity")
            .WithDescription("Sets a basket line's quantity to an absolute value - the cart page's +/- stepper.");
    }

    private static async Task<IResult> Handle(ICurrentUserAccessor currentUser, Guid id, [FromBody] SetBasketItemQuantityRequest request, ISender sender, HttpContext httpContext, IGuestIdAccessor guestIdAccessor)
    {
        var (userId, guestId) = BasketOwnerResolver.Resolve(currentUser, guestIdAccessor);

        var result = await sender.Send(new SetBasketItemQuantityCommand(userId, guestId, id, request.Quantity));
        return result.ToEnvelopedResult(httpContext);
    }
}

public record SetBasketItemQuantityRequest(int Quantity);
