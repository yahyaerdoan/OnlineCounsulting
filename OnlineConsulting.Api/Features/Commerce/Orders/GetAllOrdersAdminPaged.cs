using Core.PersistenceLayer.Dynamics.Dynamic;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.GetAllOrdersAdminPaged;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetAllUsers;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Facade;
using PageRequest = Core.ApplicationLayer.Requests.Page.PageRequest;

namespace OnlineConsulting.Api.Features.Commerce.Orders;

// AdminOrderResponse reused from GetAllOrdersAdmin.cs - same namespace, same shape, no need to redeclare.

public class GetAllOrdersAdminPaged : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/orders/admin/query", Handle)
            .WithTags("Commerce/Orders")
            .RequireAuthorization()
            .WithName("GetAllOrdersAdminPaged")
            .WithDescription("Returns every user's orders (Super Admin only), paginated (?index=&size=), optionally filtered/sorted via a DynamicQuery body, with per-order totals and basic owner display info.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext, [AsParameters] ListQueryParameters query, [FromBody] DynamicQuery? dynamicQuery)
    {
        var ordersResult = await sender.Send(new GetAllOrdersAdminPagedQuery(query.ToPageRequest(), dynamicQuery));
        if (!ordersResult.IsSuccessful || ordersResult.Data is null)
        {
            return ordersResult.ToEnvelopedResult(httpContext);
        }

        // Unbounded - this is a lookup for every order's owner, not a paged listing of its own.
        var usersResult = await sender.Send(new GetAllUsersQuery(new PageRequest { PageIndex = 0, PageSize = int.MaxValue }));
        var usersById = (usersResult.IsSuccessful ? usersResult.Data?.Items : null)?.ToDictionary(u => u.Id) ?? [];

        var responseItems = ordersResult.Data.Items.Select(o =>
        {
            _ = usersById.TryGetValue(o.UserId, out var user);
            return new AdminOrderResponse(o.Id, o.OrderNumber, o.OrderStatus, o.PaymentStatus, o.TotalPrice, o.CreatedDate, o.UserId, user?.Email, user?.UserName);
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
