using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.GetOrderStats;
using OnlineConsulting.SharedKernel.CurrentUser;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Commerce.Orders;

public class GetOrderStats : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/orders/stats", Handle)
            .WithTags("Commerce/Orders")
            .RequireAuthorization()
            .WithName("GetOrderStats")
            .WithDescription("Returns aggregate order stats (total orders, total spent) for the current user.");
    }

    private static async Task<IResult> Handle(ICurrentUserAccessor currentUser, ISender sender, HttpContext httpContext)
        => (await sender.Send(new GetOrderStatsQuery(currentUser.RequiredId())))
            .ToEnvelopedResult(httpContext);
}
