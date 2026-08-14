using OnlineConsulting.Modules.Commerce.Domain;

namespace OnlineConsulting.Modules.Commerce.Application.Common;

// Shared by AddBasketItem/RemoveBasketItem/ClearBasket - all three need to re-derive the basket's
// own totals from its current items after a write. An empty item list naturally yields zero
// totals, so ClearBasket needs no special-cased "reset to zero" branch.
public static class BasketTotalsCalculator
{
    public static (int Quantity, decimal SubTotalPrice, decimal TotalPrice) Calculate(IEnumerable<BasketItem> items)
    {
        var itemList = items as ICollection<BasketItem> ?? [.. items];
        return (itemList.Sum(i => i.Quantity), itemList.Sum(i => i.SubTotalPrice), itemList.Sum(i => i.TotalPrice));
    }
}
