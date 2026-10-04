using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Commerce.Domain;

/// <summary>One charge on an invoice. Amounts are stored at issue time, so later price or tax changes never rewrite it.</summary>
public class InvoiceLine : SequentialGuidTenantEntity
{
    private InvoiceLine()
    {
    }

    public Guid InvoiceId { get; private set; }
    public int SortOrder { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public decimal Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public int TaxRate { get; private set; }
    public decimal Subtotal { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal Total { get; private set; }

    internal static InvoiceLine Create(Guid invoiceId, int sortOrder, InvoiceCharge charge, decimal discountPercent)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(charge.Description);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(charge.Quantity);
        ArgumentOutOfRangeException.ThrowIfNegative(charge.UnitPrice);
        ArgumentOutOfRangeException.ThrowIfNegative(charge.TaxRate);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(charge.TaxRate, 100);

        var subtotal = Round(charge.Quantity * charge.UnitPrice);
        var discount = Round(subtotal * discountPercent / 100m);
        var tax = Round((subtotal - discount) * charge.TaxRate / 100m);

        return new InvoiceLine
        {
            InvoiceId = invoiceId,
            SortOrder = sortOrder,
            Description = charge.Description.Trim(),
            Quantity = charge.Quantity,
            UnitPrice = charge.UnitPrice,
            TaxRate = charge.TaxRate,
            Subtotal = subtotal,
            DiscountAmount = discount,
            TaxAmount = tax,
            Total = subtotal - discount + tax,
        };
    }

    private static decimal Round(decimal amount) => Math.Round(amount, 2, MidpointRounding.AwayFromZero);
}
