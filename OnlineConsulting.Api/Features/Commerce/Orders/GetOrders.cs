using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.GetOrders;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Commerce.Orders;

public class GetOrders : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/orders", Handle)
            .WithTags("Commerce/Orders")
            .RequireAuthorization()
            .WithName("GetOrders")
            .WithDescription("Returns the current user's orders, newest first, with per-order totals.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext)
        => (await sender.Send(new GetCurrentUserQuery())
                .BindAsync(user => sender.Send(new GetOrdersQuery(user.Id))))
            .ToEnvelopedResult(httpContext);
}
