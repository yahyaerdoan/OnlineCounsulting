using OnlineConsulting.Modules.Commerce.Domain;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Contracts;

public record BasketItemResponse(
    Guid Id,
    Guid ServiceId,
    int Quantity,
    decimal Price,
    int TaxRate,
    decimal TaxAmount,
    decimal SubTotalPrice,
    decimal TotalPrice)
{
    public static BasketItemResponse FromDomain(BasketItem item) => new(
        item.Id, item.ServiceId, item.Quantity, item.Price, item.TaxRate, item.TaxAmount, item.SubTotalPrice, item.TotalPrice);
}
