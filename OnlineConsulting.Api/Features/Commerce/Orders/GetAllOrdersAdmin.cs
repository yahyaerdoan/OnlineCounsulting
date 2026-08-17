using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.GetAllOrdersAdmin;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetAllUsers;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Facade;

namespace OnlineConsulting.Api.Features.Commerce.Orders;

/// <summary>Admin-scoped list of every user's orders, with basic owner display info joined in from Identity.</summary>
public record AdminOrderResponse(Guid Id, string OrderNumber, string OrderStatus, string PaymentStatus, decimal TotalPrice, DateTimeOffset CreatedDate, Guid UserId, string? UserEmail, string? UserName);

public class GetAllOrdersAdmin : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/orders/admin", Handle)
            .WithTags("Commerce/Orders")
            .RequireAuthorization()
            .WithName("GetAllOrdersAdmin")
            .WithDescription("Returns every user's orders (Super Admin only), with per-order totals and basic owner display info.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext)
    {
        var ordersResult = await sender.Send(new GetAllOrdersAdminQuery());
        if (!ordersResult.IsSuccessful || ordersResult.Data is null)
            return ordersResult.ToEnvelopedResult(httpContext);

        var usersResult = await sender.Send(new GetAllUsersQuery());
        var usersById = (usersResult.IsSuccessful ? usersResult.Data : null)?.ToDictionary(u => u.Id) ?? [];

        var responses = ordersResult.Data.Select(o =>
        {
            usersById.TryGetValue(o.UserId, out var user);
            return new AdminOrderResponse(o.Id, o.OrderNumber, o.OrderStatus, o.PaymentStatus, o.TotalPrice, o.CreatedDate, o.UserId, user?.Email, user?.UserName);
        }).ToList();

        return Result.Success(responses, "Orders retrieved successfully.").ToEnvelopedResult(httpContext);
    }
}
