namespace OnlineConsulting.SharedKernel.Catalog;

/// <summary>Cross-module read of the current tenant's service catalog (implemented by the Services module).</summary>
public interface IServiceCatalogReader
{
    /// <summary>Null when the service isn't in the current tenant's catalog.</summary>
    Task<ServiceCatalogEntry?> GetAsync(Guid serviceId, CancellationToken cancellationToken = default);

    /// <summary>Entries keyed by service id; ids not in the catalog are left out.</summary>
    Task<IReadOnlyDictionary<Guid, ServiceCatalogEntry>> GetManyAsync(IEnumerable<Guid> serviceIds, CancellationToken cancellationToken = default);
}
