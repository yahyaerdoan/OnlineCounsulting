using OnlineConsulting.Modules.Commerce.Domain;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Orders.Contracts;

public record OrderItemResponse(
    Guid Id,
    Guid ServiceId,
    int Quantity,
    decimal UnitPrice,
    int TaxRate,
    decimal TaxAmount,
    decimal SubTotalPrice,
    decimal TotalPrice)
{
    public static OrderItemResponse FromDomain(OrderItem item) => new(
        item.Id, item.ServiceId, item.Quantity, item.UnitPrice, item.TaxRate, item.TaxAmount, item.SubTotalPrice, item.TotalPrice);
}
