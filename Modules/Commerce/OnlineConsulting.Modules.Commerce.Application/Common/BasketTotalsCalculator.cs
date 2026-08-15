using OnlineConsulting.Modules.Commerce.Domain;

namespace OnlineConsulting.Modules.Commerce.Application.Common;

/// <summary>Re-derives a basket's totals from its current items, shared by AddBasketItem/RemoveBasketItem/ClearBasket after a write.</summary>
public static class BasketTotalsCalculator
{
    public static (int Quantity, decimal SubTotalPrice, decimal TotalPrice) Calculate(IEnumerable<BasketItem> items)
    {
        var itemList = items as ICollection<BasketItem> ?? [.. items];
        return (itemList.Sum(i => i.Quantity), itemList.Sum(i => i.SubTotalPrice), itemList.Sum(i => i.TotalPrice));
    }
}
