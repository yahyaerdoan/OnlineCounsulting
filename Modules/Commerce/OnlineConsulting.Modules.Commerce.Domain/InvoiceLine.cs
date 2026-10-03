using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Commerce.Domain;

/// <summary>One charge on an invoice. Amounts are stored, not recomputed, so a later tax or price change never rewrites an issued invoice.</summary>
public class InvoiceLine : SequentialGuidTenantEntity
{
    public required Guid InvoiceId { get; set; }
    public required int SortOrder { get; set; }
    public required string Description { get; set; }
    public required decimal Quantity { get; set; }
    public required decimal UnitPrice { get; set; }
    public required int TaxRate { get; set; }
    public decimal Subtotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal Total { get; set; }
}
