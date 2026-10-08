using Core.PersistenceLayer.MultiTenancy;

namespace OnlineConsulting.Modules.Commerce.Domain;

/// <summary>One purchased service on an order, created only by <see cref="Order.Place"/>. Price and tax are copied at checkout, so later catalog changes never rewrite it.</summary>
public class OrderItem : TenantEntity<Guid>
{
    private OrderItem()
    {
    }

    public Guid OrderId { get; private set; }

    /// <summary>Services module id, no navigation.</summary>
    public Guid ServiceId { get; private set; }

    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public int TaxRate { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal SubTotalPrice { get; private set; }
    public decimal TotalPrice { get; private set; }

    internal static OrderItem Create(Guid orderId, Guid serviceId, int quantity, decimal unitPrice, int taxRate)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
        ArgumentOutOfRangeException.ThrowIfNegative(unitPrice);
        ArgumentOutOfRangeException.ThrowIfNegative(taxRate);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(taxRate, 100);

        var item = new OrderItem { OrderId = orderId, ServiceId = serviceId, Quantity = quantity, UnitPrice = unitPrice, TaxRate = taxRate };
        (item.SubTotalPrice, item.TaxAmount, item.TotalPrice) = LineAmounts.Calculate(unitPrice, quantity, taxRate);
        return item;
    }
}
