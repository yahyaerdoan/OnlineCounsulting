using OnlineConsulting.Modules.Commerce.Domain;
using OnlineConsulting.SharedKernel.Identity;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Orders.Contracts;

/// <summary>An order in the staff list: the OrderResponse fields plus the owner's id, email and user name.</summary>
public record AdminOrderResponse(Guid Id, string OrderNumber, string OrderStatus, string PaymentStatus, decimal TotalPrice, DateTimeOffset CreatedDate, Guid UserId,
    string? UserEmail, string? UserName)
{
    public static AdminOrderResponse FromDomain(Order order, decimal totalPrice, UserContact? owner) => new(
        order.Id, order.OrderNumber, order.OrderStatus, order.PaymentStatus, totalPrice, order.CreatedDate, order.UserId, owner?.Email, owner?.UserName);
}
