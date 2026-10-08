using OnlineConsulting.Modules.Commerce.Domain;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Contracts;

public record BasketResponse(Guid Id, int Quantity, decimal SubTotalPrice, decimal TotalPrice, IReadOnlyList<BasketItemResponse> Items)
{
    public static BasketResponse FromDomain(Basket basket) => new(
        basket.Id, basket.Quantity, basket.SubTotalPrice, basket.TotalPrice, [.. basket.Items.Select(BasketItemResponse.FromDomain)]);
}
