using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Contracts;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.ListOrders;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Commerce.Orders;

public class ListOrders : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/orders/admin/query", Handle)
            .WithTags("Commerce/Orders")
            .RequireAuthorization()
            .WithName("ListOrders")
            .WithDescription("Returns every user's orders (Super Admin only), paginated (?index=&size=), optionally filtered/sorted via a DynamicQuery body, with per-order totals and the owner's email and user name for the orders on the page.")
            .ProducesEnveloped<Paginate<AdminOrderResponse>>();
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext, [AsParameters] ListQueryParameters query, [FromBody] DynamicQueryRequest? dynamicQuery)
        => (await sender.Send(new ListOrdersQuery(query.ToPageRequest(), dynamicQuery?.ToDynamicQuery()))).ToEnvelopedResult(httpContext);
}
