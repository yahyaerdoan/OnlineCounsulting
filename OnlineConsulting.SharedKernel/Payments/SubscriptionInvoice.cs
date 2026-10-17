namespace OnlineConsulting.SharedKernel.Payments;

/// <summary>A provider-issued subscription invoice. The provider stays the system of record (its numbering, PDF and hosted page); we only relay it.
/// BillingReason "subscription_create" marks the first invoice and "subscription_update" a mid-period module charge; both are receipted when they happen.</summary>
public sealed record SubscriptionInvoice(string ProviderInvoiceId, string? Number, decimal AmountPaid, string Currency, string Status,
    string? HostedUrl, string? PdfUrl, DateTimeOffset? PeriodStart, DateTimeOffset? PeriodEnd, string? BillingReason, IReadOnlyList<SubscriptionInvoiceLine> Lines)
{
    public const string FirstInvoiceReason = "subscription_create";
    public const string ModuleChangeReason = "subscription_update";

    /// <summary>Receipted by the signup or the module change itself, not by the renewal webhook.</summary>
    public bool IsReceiptedOnPurchase => BillingReason is FirstInvoiceReason or ModuleChangeReason;

    public bool IsPaid => Status == "paid";
}
