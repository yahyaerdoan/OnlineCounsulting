using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Commerce.Domain;

/// <summary>The billing record for one paid order or one completed visit. Customer and address details are copied in at issue time,
/// so the invoice keeps saying what was billed even if the account or address changes later.</summary>
public class Invoice : SequentialGuidTenantEntity
{
    public required string InvoiceNumber { get; set; }

    /// <summary>Plain id, no navigation - User lives in the Identity module's own DbContext.</summary>
    public required Guid UserId { get; set; }

    /// <summary>Order or Appointment - see InvoiceSources.</summary>
    public required string SourceType { get; set; }

    public required Guid SourceId { get; set; }

    /// <summary>Open, Paid or Void - see InvoiceStatuses.</summary>
    public required string Status { get; set; }

    public required string Currency { get; set; }
    public required string BillToName { get; set; }
    public string? BillToEmail { get; set; }
    public string? BillToAddress { get; set; }

    /// <summary>"Service visit: Duct cleaning" or "Order ORD-1A2B3C4D" - the one-line reason shown in lists and emails.</summary>
    public required string Title { get; set; }

    public decimal Subtotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public string? DiscountLabel { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal Total { get; set; }

    public required DateTimeOffset IssuedAt { get; set; }
    public DateTimeOffset? DueAt { get; set; }
    public DateTimeOffset? PaidAt { get; set; }

    /// <summary>Card, Cash, Check or Covered - how a Paid invoice was settled.</summary>
    public string? PaymentMethod { get; set; }
    public string? PaymentProvider { get; set; }
    public string? ProviderPaymentId { get; set; }
    public string? VoidReason { get; set; }
}
