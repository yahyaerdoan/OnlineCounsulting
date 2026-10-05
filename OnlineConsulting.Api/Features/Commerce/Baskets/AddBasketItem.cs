using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.AddBasketItem;
using OnlineConsulting.SharedKernel.CurrentUser;
using OnlineConsulting.SharedKernel.GuestIdentity;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Commerce.Baskets;

public class AddBasketItem : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/basket/items", Handle)
            .WithTags("Commerce/Baskets")
            .WithName("AddBasketItem")
            .WithDescription("Adds a service to the current user's (or guest's) basket, increasing its quantity if already present.")
            .ProducesEnveloped(StatusCodes.Status201Created);
    }

    private static async Task<IResult> Handle(ICurrentUserAccessor currentUser, [FromBody] AddBasketItemRequest request, ISender sender, HttpContext httpContext, IGuestIdAccessor guestIdAccessor)
    {
        var (userId, guestId) = BasketOwnerResolver.Resolve(currentUser, guestIdAccessor);

        var result = await sender.Send(request.ToCommand(userId, guestId));
        return result.ToEnvelopedResult(httpContext);
    }
}

public record AddBasketItemRequest(Guid ServiceId, int Quantity)
{
    public AddBasketItemCommand ToCommand(Guid? userId, Guid? guestId) => new(userId, guestId, ServiceId, Quantity);
}
