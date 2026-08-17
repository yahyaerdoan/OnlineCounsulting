using OnlineConsulting.Modules.Commerce.Domain;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Orders.Contracts;

/// <summary>Same shape as OrderResponse plus the owning user's id - Commerce has no reference to Identity, so email/username enrichment happens at the Api layer.</summary>
public record AdminOrderResponse(Guid Id, string OrderNumber, string OrderStatus, string PaymentStatus, decimal TotalPrice, DateTimeOffset CreatedDate, Guid UserId)
{
    public static AdminOrderResponse FromDomain(Order order, decimal totalPrice) => new(
        order.Id, order.OrderNumber, order.OrderStatus, order.PaymentStatus, totalPrice, order.CreatedDate, order.UserId);
}
