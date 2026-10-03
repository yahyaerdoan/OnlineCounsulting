namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

public record InvoiceLineResponse(string Description, decimal Quantity, decimal UnitPrice, int TaxRate, decimal Subtotal, decimal DiscountAmount, decimal TaxAmount, decimal Total);

/// <summary>Mirrors Commerce's InvoiceResponse - Lines is empty in lists and filled for a single invoice.</summary>
public record InvoiceResponse(
    Guid Id, string InvoiceNumber, Guid UserId, string SourceType, Guid SourceId, string Status, string Currency, string Title,
    string BillToName, string? BillToEmail, string? BillToAddress,
    decimal Subtotal, decimal DiscountAmount, string? DiscountLabel, decimal TaxAmount, decimal Total,
    DateTimeOffset IssuedAt, DateTimeOffset? DueAt, DateTimeOffset? PaidAt, string? PaymentMethod, string? VoidReason,
    List<InvoiceLineResponse> Lines) : IQueryableFields
{
    public static string[] SearchFields => [nameof(InvoiceNumber), nameof(BillToName), nameof(Status)];

    public bool IsOpen => Status == "Open";

    public bool IsPaid => Status == "Paid";

    public bool IsOverdue => IsOpen && DueAt is { } due && due < DateTimeOffset.UtcNow;
}

public record PayInvoiceResult(bool Paid, string? ClientSecret);

/// <summary>Mirrors Commerce's SyncInvoicePaymentResult.</summary>
public record SyncInvoicePaymentResult(string Status, bool Paid);

public record InvoicePdfResponse(string FileName, string ContentBase64);
