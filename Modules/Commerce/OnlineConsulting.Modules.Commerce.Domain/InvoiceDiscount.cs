namespace OnlineConsulting.Modules.Commerce.Domain;

/// <summary>A percentage taken off every line's subtotal, shown on the invoice as <paramref name="Label"/>.</summary>
public sealed record InvoiceDiscount(decimal Percent, string Label);
