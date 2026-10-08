using Core.PersistenceLayer.MultiTenancy;

namespace OnlineConsulting.Modules.Commerce.Domain;

/// <summary>One line of a <see cref="Basket"/>; created and changed only through the basket. Its id is assigned when it is saved.</summary>
public class BasketItem : TenantEntity<Guid>
{
    private BasketItem()
    {
    }

    public Guid BasketId { get; private set; }

    /// <summary>Catalog service id, no navigation.</summary>
    public Guid ServiceId { get; private set; }

    public int Quantity { get; private set; }
    public decimal Price { get; private set; }
    public int TaxRate { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal SubTotalPrice { get; private set; }
    public decimal TotalPrice { get; private set; }

    internal static BasketItem Create(Guid basketId, Guid serviceId, int quantity, decimal unitPrice, int taxRate)
    {
        var item = new BasketItem { BasketId = basketId, ServiceId = serviceId };
        item.SetPrice(unitPrice, taxRate);
        item.SetQuantity(quantity);
        return item;
    }

    internal void SetQuantity(int quantity)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
        Quantity = quantity;
        Recalculate();
    }

    internal void SetPrice(decimal unitPrice, int taxRate)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(unitPrice);
        ArgumentOutOfRangeException.ThrowIfNegative(taxRate);
        Price = unitPrice;
        TaxRate = taxRate;
        Recalculate();
    }

    private void Recalculate() => (SubTotalPrice, TaxAmount, TotalPrice) = LineAmounts.Calculate(Price, Quantity, TaxRate);
}
