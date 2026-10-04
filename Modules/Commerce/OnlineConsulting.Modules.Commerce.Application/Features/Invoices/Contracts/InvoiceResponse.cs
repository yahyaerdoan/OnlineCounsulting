using OnlineConsulting.Modules.Commerce.Domain;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Contracts;

/// <summary>Lines are empty for list queries and filled for a single invoice.</summary>
public sealed record InvoiceResponse(
    Guid Id, string InvoiceNumber, Guid UserId, string SourceType, Guid SourceId, string Status, string Currency, string Title,
    string BillToName, string? BillToEmail, string? BillToAddress,
    decimal Subtotal, decimal DiscountAmount, string? DiscountLabel, decimal TaxAmount, decimal Total,
    DateTimeOffset IssuedAt, DateTimeOffset? DueAt, DateTimeOffset? PaidAt, string? PaymentMethod, string? VoidReason,
    IReadOnlyList<InvoiceLineResponse> Lines)
{
    /// <summary>Lines are listed only when the invoice was loaded with them; list queries leave them out.</summary>
    public static InvoiceResponse FromDomain(Invoice invoice) => new(
        invoice.Id, invoice.InvoiceNumber, invoice.UserId, invoice.SourceType, invoice.SourceId, invoice.Status, invoice.Currency, invoice.Title,
        invoice.BillToName, invoice.BillToEmail, invoice.BillToAddress,
        invoice.Subtotal, invoice.DiscountAmount, invoice.DiscountLabel, invoice.TaxAmount, invoice.Total,
        invoice.IssuedAt, invoice.DueAt, invoice.PaidAt, invoice.PaymentMethod, invoice.VoidReason,
        [.. invoice.Lines.OrderBy(l => l.SortOrder).Select(InvoiceLineResponse.FromDomain)]);
}
