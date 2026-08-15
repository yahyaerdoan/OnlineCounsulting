using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.CreateOrderFromBasket;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Commerce.Orders;

public class CreateOrderFromBasket : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/orders/checkout", Handle)
            .WithTags("Commerce/Orders")
            .RequireAuthorization()
            .WithName("CreateOrderFromBasket")
            .WithDescription("Checks out the current user's basket into a new order, using their current shipping/billing address.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext)
    {
        var currentUser = await sender.Send(new GetCurrentUserQuery());
        if (!currentUser.IsSuccessful || currentUser.Data is null)
            return currentUser.ToEnvelopedResult(httpContext);

        var result = await sender.Send(new CreateOrderFromBasketCommand(currentUser.Data.Id, currentUser.Data.Email));
        return result.ToEnvelopedResult(httpContext);
    }
}
