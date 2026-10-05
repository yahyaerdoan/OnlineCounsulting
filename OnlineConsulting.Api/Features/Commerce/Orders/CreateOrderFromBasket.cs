using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Contracts;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.CreateOrderFromBasket;
using OnlineConsulting.SharedKernel.CurrentUser;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Commerce.Orders;

public class CreateOrderFromBasket : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/orders/checkout", Handle)
            .WithTags("Commerce/Orders")
            .RequireAuthorization()
            .WithName("CreateOrderFromBasket")
            .WithDescription("Checks out the current user's basket into a new order, using their current shipping/billing address.")
            .ProducesEnveloped<CreateOrderResult>(StatusCodes.Status201Created);
    }

    private static async Task<IResult> Handle(ICurrentUserAccessor currentUser, ISender sender, HttpContext httpContext)
        => (await sender.Send(new CreateOrderFromBasketCommand(currentUser.RequiredId(), currentUser.RequiredEmail())))
            .ToEnvelopedResult(httpContext);
}
