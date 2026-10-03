namespace OnlineConsulting.SharedKernel.Catalog;

/// <summary>The catalog facts other modules must trust instead of client input: what a service is and what it costs.
/// UnitPrice is the price actually charged (the discounted price).</summary>
public sealed record ServiceCatalogEntry(Guid Id, string Title, string Kind, decimal UnitPrice, int TaxRate);

/// <summary>Cross-module read of the current tenant's service catalog (implemented by the Services module).</summary>
public interface IServiceCatalogReader
{
    Task<ServiceCatalogEntry?> GetAsync(Guid serviceId, CancellationToken cancellationToken = default);

    Task<IReadOnlyDictionary<Guid, ServiceCatalogEntry>> GetManyAsync(IEnumerable<Guid> serviceIds, CancellationToken cancellationToken = default);
}
