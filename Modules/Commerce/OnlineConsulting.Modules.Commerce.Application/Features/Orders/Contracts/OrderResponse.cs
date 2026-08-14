using OnlineConsulting.Modules.Commerce.Domain;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Orders.Contracts;

public record OrderResponse(Guid Id, string OrderNumber, string OrderStatus, string PaymentStatus, decimal TotalPrice, DateTimeOffset CreatedDate)
{
    public static OrderResponse FromDomain(Order order, decimal totalPrice) => new(
        order.Id, order.OrderNumber, order.OrderStatus, order.PaymentStatus, totalPrice, order.CreatedDate);
}
