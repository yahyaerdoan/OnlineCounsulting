using OnlineConsulting.Modules.Commerce.Domain;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Invoices;

/// <summary>Per line: subtotal = quantity x unit price, the member discount comes off the subtotal, tax is charged on what's left.
/// Every amount is rounded to cents half away from zero, and the invoice totals are the sums of the rounded line amounts.</summary>
public static class InvoiceCalculator
{
    public static decimal Round(decimal amount) => Math.Round(amount, 2, MidpointRounding.AwayFromZero);

    public static void ApplyLine(InvoiceLine line, decimal discountPercent)
    {
        line.Subtotal = Round(line.Quantity * line.UnitPrice);
        line.DiscountAmount = Round(line.Subtotal * discountPercent / 100m);
        line.TaxAmount = Round((line.Subtotal - line.DiscountAmount) * line.TaxRate / 100m);
        line.Total = line.Subtotal - line.DiscountAmount + line.TaxAmount;
    }

    public static void ApplyTotals(Invoice invoice, IReadOnlyCollection<InvoiceLine> lines)
    {
        invoice.Subtotal = lines.Sum(l => l.Subtotal);
        invoice.DiscountAmount = lines.Sum(l => l.DiscountAmount);
        invoice.TaxAmount = lines.Sum(l => l.TaxAmount);
        invoice.Total = lines.Sum(l => l.Total);
    }

    public static string NewNumber(DateTimeOffset issuedAt) =>
        $"INV-{issuedAt:yyMM}-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}";
}
