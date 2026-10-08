namespace OnlineConsulting.SharedKernel.Catalog;

/// <summary>The catalog facts other modules must trust instead of client input: what a service is and what it costs.
/// UnitPrice is the price actually charged (the discounted price).</summary>
public sealed record ServiceCatalogEntry(Guid Id, string Title, string Kind, decimal UnitPrice, int TaxRate);
