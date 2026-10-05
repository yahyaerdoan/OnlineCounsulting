namespace OnlineConsulting.SharedKernel.Payments;

/// <summary>A provider-issued subscription invoice. The provider stays the system of record (its numbering, PDF and hosted page); we only relay it.
/// BillingReason "subscription_create" marks the first invoice, already receipted at signup.</summary>
public sealed record SubscriptionInvoice(string ProviderInvoiceId, string? Number, decimal AmountPaid, string Currency, string Status,
    string? HostedUrl, string? PdfUrl, DateTimeOffset? PeriodStart, DateTimeOffset? PeriodEnd, string? BillingReason, IReadOnlyList<SubscriptionInvoiceLine> Lines)
{
    public const string FirstInvoiceReason = "subscription_create";

    public bool IsPaid => Status == "paid";
}
