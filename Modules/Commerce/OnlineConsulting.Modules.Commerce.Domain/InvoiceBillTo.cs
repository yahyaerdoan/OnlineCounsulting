namespace OnlineConsulting.Modules.Commerce.Domain;

/// <summary>Customer details copied onto an <see cref="Invoice"/> at issue time.</summary>
public sealed record InvoiceBillTo(string Name, string? Email, string? Address);
