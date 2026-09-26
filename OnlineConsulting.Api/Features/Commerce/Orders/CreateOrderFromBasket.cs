using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.CreateOrderFromBasket;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Commerce.Orders;

public class CreateOrderFromBasket : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/orders/checkout", Handle)
            .WithTags("Commerce/Orders")
            .RequireAuthorization()
            .WithName("CreateOrderFromBasket")
            .WithDescription("Checks out the current user's basket into a new order, using their current shipping/billing address.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext)
        => (await sender.Send(new GetCurrentUserQuery())
                .BindAsync(user => sender.Send(new CreateOrderFromBasketCommand(user.Id, user.Email))))
            .ToEnvelopedResult(httpContext);
}
