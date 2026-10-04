using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.GetOrders;
using OnlineConsulting.SharedKernel.CurrentUser;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Commerce.Orders;

public class GetOrders : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/orders", Handle)
            .WithTags("Commerce/Orders")
            .RequireAuthorization()
            .WithName("GetOrders")
            .WithDescription("Returns the current user's orders, newest first, with per-order totals.");
    }

    private static async Task<IResult> Handle(ICurrentUserAccessor currentUser, ISender sender, HttpContext httpContext)
        => (await sender.Send(new GetOrdersQuery(currentUser.RequiredId())))
            .ToEnvelopedResult(httpContext);
}
