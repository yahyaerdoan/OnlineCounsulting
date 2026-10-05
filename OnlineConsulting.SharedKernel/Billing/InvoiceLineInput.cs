namespace OnlineConsulting.SharedKernel.Billing;

/// <summary>One billable line of a completed visit; TaxRate is a whole percent.</summary>
public sealed record InvoiceLineInput(string Description, decimal Quantity, decimal UnitPrice, int TaxRate);
