using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Commerce.Domain;

/// <summary>
/// A user's or guest's cart and its lines. Lines change only through its methods, which keep the basket totals in step with them;
/// load and save it together with <see cref="Items"/>.
/// </summary>
public class Basket : SequentialGuidTenantEntity
{
    private readonly List<BasketItem> _items = [];

    private Basket()
    {
    }

    /// <summary>Exactly one of UserId and GuestId is set.</summary>
    public Guid? UserId { get; private set; }
    public Guid? GuestId { get; private set; }

    public int Quantity { get; private set; }
    public decimal SubTotalPrice { get; private set; }
    public decimal TotalPrice { get; private set; }

    public IReadOnlyList<BasketItem> Items => _items;

    public bool IsEmpty => _items.Count == 0;

    /// <summary>An empty basket for a signed-in user or a guest; exactly one of the two must be given.</summary>
    public static Basket Open(Guid? userId, Guid? guestId)
    {
        if (userId.HasValue == guestId.HasValue)
        {
            throw new ArgumentException("A basket belongs to exactly one of a user or a guest.");
        }

        return new Basket { UserId = userId, GuestId = guestId };
    }

    /// <summary>The line with this id, or null.</summary>
    public BasketItem? FindItem(Guid basketItemId) => _items.Find(i => i.Id == basketItemId);

    /// <summary>Adds <paramref name="quantity"/> of a service at its current price; an existing line for it grows and takes the new price.</summary>
    public void AddItem(Guid serviceId, int quantity, decimal unitPrice, int taxRate)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
        if (_items.Find(i => i.ServiceId == serviceId) is { } existing)
        {
            existing.SetPrice(unitPrice, taxRate);
            existing.SetQuantity(existing.Quantity + quantity);
        }
        else
        {
            _items.Add(BasketItem.Create(Id, serviceId, quantity, unitPrice, taxRate));
        }

        RecalculateTotals();
    }

    /// <summary>Sets a line's quantity; the line must be in this basket.</summary>
    public void SetItemQuantity(Guid basketItemId, int quantity)
    {
        GetItem(basketItemId).SetQuantity(quantity);
        RecalculateTotals();
    }

    /// <summary>Removes a line; the line must be in this basket.</summary>
    public void RemoveItem(Guid basketItemId)
    {
        _ = _items.Remove(GetItem(basketItemId));
        RecalculateTotals();
    }

    /// <summary>Updates the price of the line for <paramref name="serviceId"/>, if any (checkout re-prices from the catalog).</summary>
    public void Reprice(Guid serviceId, decimal unitPrice, int taxRate)
    {
        if (_items.Find(i => i.ServiceId == serviceId) is { } item)
        {
            item.SetPrice(unitPrice, taxRate);
            RecalculateTotals();
        }
    }

    public void Clear()
    {
        _items.Clear();
        RecalculateTotals();
    }

    /// <summary>Moves a guest basket's lines into this one after sign-in: matching lines add up and keep this basket's price, and the guest basket is emptied.</summary>
    public void MergeFrom(Basket guestBasket)
    {
        if (guestBasket.Id == Id)
        {
            throw new InvalidOperationException($"Basket {Id} cannot be merged into itself.");
        }

        foreach (var guestItem in guestBasket._items)
        {
            if (_items.Find(i => i.ServiceId == guestItem.ServiceId) is { } existing)
            {
                existing.SetQuantity(existing.Quantity + guestItem.Quantity);
            }
            else
            {
                _items.Add(BasketItem.Create(Id, guestItem.ServiceId, guestItem.Quantity, guestItem.Price, guestItem.TaxRate));
            }
        }

        guestBasket.Clear();
        RecalculateTotals();
    }

    private BasketItem GetItem(Guid basketItemId) =>
        FindItem(basketItemId) ?? throw new InvalidOperationException($"Basket {Id} has no item {basketItemId}.");

    private void RecalculateTotals()
    {
        Quantity = _items.Sum(i => i.Quantity);
        SubTotalPrice = _items.Sum(i => i.SubTotalPrice);
        TotalPrice = _items.Sum(i => i.TotalPrice);
    }
}
