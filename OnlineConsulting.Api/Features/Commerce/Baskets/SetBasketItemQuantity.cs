using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.SetBasketItemQuantity;
using OnlineConsulting.SharedKernel.GuestIdentity;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Commerce.Baskets;

public class SetBasketItemQuantity : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/api/basket/items/{id:guid}", Handle)
            .WithTags("Commerce/Baskets")
            .WithName("SetBasketItemQuantity")
            .WithDescription("Sets a basket line's quantity to an absolute value - the cart page's +/- stepper.");
    }

    private static async Task<IResult> Handle(Guid id, [FromBody] SetBasketItemQuantityRequest request, ISender sender, HttpContext httpContext, IGuestIdAccessor guestIdAccessor)
    {
        var (userId, guestId, error) = await BasketOwnerResolver.ResolveAsync(sender, httpContext, guestIdAccessor);

        if (error is not null)
        {
            return error;
        }

        var result = await sender.Send(new SetBasketItemQuantityCommand(userId, guestId, id, request.Quantity));
        return result.ToEnvelopedResult(httpContext);
    }
}

public record SetBasketItemQuantityRequest(int Quantity);
