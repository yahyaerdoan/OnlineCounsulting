namespace OnlineConsulting.Modules.Commerce.Application.Features.Invoices;

public static class InvoiceNumberGenerator
{
    public static string Generate(DateTimeOffset issuedAt) => $"INV-{issuedAt:yyMM}-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}";
}
