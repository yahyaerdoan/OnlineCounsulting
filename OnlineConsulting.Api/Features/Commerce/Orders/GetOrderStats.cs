using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.GetOrderStats;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Commerce.Orders;

public class GetOrderStats : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/orders/stats", Handle)
            .WithTags("Commerce/Orders")
            .RequireAuthorization()
            .WithName("GetOrderStats")
            .WithDescription("Returns aggregate order stats (total orders, total spent) for the current user.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext)
        => (await sender.Send(new GetCurrentUserQuery())
                .BindAsync(user => sender.Send(new GetOrderStatsQuery(user.Id))))
            .ToEnvelopedResult(httpContext);
}
