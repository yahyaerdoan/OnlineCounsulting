namespace OnlineConsulting.Modules.Commerce.Domain;

/// <summary>One thing to bill when issuing an <see cref="Invoice"/>; becomes an <see cref="InvoiceLine"/>.</summary>
public sealed record InvoiceCharge(string Description, decimal Quantity, decimal UnitPrice, int TaxRate);
