using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Abstractions;
using OnlineConsulting.Modules.Commerce.Domain;
using OnlineConsulting.SharedKernel.Persistence;

namespace OnlineConsulting.Modules.Commerce.Application.Common;

/// <summary>Re-derives a basket's totals from its current items, shared by AddBasketItem/RemoveBasketItem/ClearBasket after a write.</summary>
public static class BasketTotalsCalculator
{
    public static (int Quantity, decimal SubTotalPrice, decimal TotalPrice) Calculate(IEnumerable<BasketItem> items)
    {
        var itemList = items as ICollection<BasketItem> ?? [.. items];
        return (itemList.Sum(i => i.Quantity), itemList.Sum(i => i.SubTotalPrice), itemList.Sum(i => i.TotalPrice));
    }

    /// <summary>Reloads a basket's items, recomputes its totals, and saves it - call after any basket item write.</summary>
    public static async Task RecalculateAndSaveAsync(Basket basket, IBasketItemRepository basketItemRepository, IBasketRepository basketRepository, CancellationToken cancellationToken)
    {
        var items = await basketItemRepository.GetListAsync(i => i.BasketId == basket.Id, size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);
        (basket.Quantity, basket.SubTotalPrice, basket.TotalPrice) = Calculate(items.Items);
        _ = await basketRepository.UpdateAsync(basket);
    }
}
