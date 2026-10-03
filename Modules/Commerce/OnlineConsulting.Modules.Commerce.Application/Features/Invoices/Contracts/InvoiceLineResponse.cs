using OnlineConsulting.Modules.Commerce.Domain;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Contracts;

public sealed record InvoiceLineResponse(string Description, decimal Quantity, decimal UnitPrice, int TaxRate, decimal Subtotal, decimal DiscountAmount, decimal TaxAmount, decimal Total)
{
    public static InvoiceLineResponse FromDomain(InvoiceLine line) =>
        new(line.Description, line.Quantity, line.UnitPrice, line.TaxRate, line.Subtotal, line.DiscountAmount, line.TaxAmount, line.Total);
}
