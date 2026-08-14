using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.AddBasketItem;
using OnlineConsulting.SharedKernel.GuestIdentity;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Commerce.Baskets;

public class AddBasketItem : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/basket/items", Handle)
            .WithTags("Commerce/Baskets")
            .WithName("AddBasketItem")
            .WithDescription("Adds a service to the current user's (or guest's) basket, or updates its quantity if already present.");
    }

    private static async Task<IResult> Handle([FromBody] AddBasketItemCommand command, ISender sender, HttpContext httpContext, IGuestIdAccessor guestIdAccessor)
    {
        var (userId, guestId, error) = await BasketOwnerResolver.ResolveAsync(sender, httpContext, guestIdAccessor);
        if (error is not null)
            return error;

        var result = await sender.Send(command with { UserId = userId, GuestId = guestId });
        return result.ToEnvelopedResult(httpContext);
    }
}
