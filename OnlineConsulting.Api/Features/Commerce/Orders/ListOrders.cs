using Core.PersistenceLayer.Dynamics.Dynamic;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.ListOrders;
using OnlineConsulting.SharedKernel.Identity;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Facade;

namespace OnlineConsulting.Api.Features.Commerce.Orders;

public class ListOrders : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/orders/admin/query", Handle)
            .WithTags("Commerce/Orders")
            .RequireAuthorization()
            .WithName("ListOrders")
            .WithDescription("Returns every user's orders (Super Admin only), paginated (?index=&size=), optionally filtered/sorted via a DynamicQuery body, with per-order totals and the owner's email and user name for the orders on the page.");
    }

    private static async Task<IResult> Handle(ISender sender, IUserContactReader contactReader, HttpContext httpContext, [AsParameters] ListQueryParameters query, [FromBody] DynamicQuery? dynamicQuery)
    {
        var ordersResult = await sender.Send(new ListOrdersQuery(query.ToPageRequest(), dynamicQuery));

        if (!ordersResult.IsSuccessful || ordersResult.Data is null)
        {
            return ordersResult.ToEnvelopedResult(httpContext);
        }

        var owners = (await contactReader.GetContactsAsync([.. ordersResult.Data.Items.Select(o => o.UserId).Distinct()], httpContext.RequestAborted))
            .ToDictionary(c => c.Id);

        var responseItems = ordersResult.Data.Items.Select(o =>
        {
            var owner = owners.GetValueOrDefault(o.UserId);
            return new AdminOrderResponse(o.Id, o.OrderNumber, o.OrderStatus, o.PaymentStatus, o.TotalPrice, o.CreatedDate, o.UserId, owner?.Email, owner?.UserName);
        }).ToList();

        var response = new Paginate<AdminOrderResponse>
        {
            Items = responseItems,
            Index = ordersResult.Data.Index,
            Size = ordersResult.Data.Size,
            Count = ordersResult.Data.Count,
            Pages = ordersResult.Data.Pages,
        };

        return Result.Success(response, "Orders retrieved successfully.").ToEnvelopedResult(httpContext);
    }
}
