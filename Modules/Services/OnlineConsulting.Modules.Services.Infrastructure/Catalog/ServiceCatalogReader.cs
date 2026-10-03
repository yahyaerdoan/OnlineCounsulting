using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.Services.Domain;
using OnlineConsulting.Modules.Services.Infrastructure.Persistence;
using OnlineConsulting.SharedKernel.Catalog;

namespace OnlineConsulting.Modules.Services.Infrastructure.Catalog;

/// <summary>Read-only catalog facts for other modules; the DbContext's tenant filter keeps it to the current tenant.</summary>
public sealed class ServiceCatalogReader(ServicesDbContext context) : IServiceCatalogReader
{
    public async Task<ServiceCatalogEntry?> GetAsync(Guid serviceId, CancellationToken cancellationToken = default) =>
        (await GetManyAsync([serviceId], cancellationToken)).GetValueOrDefault(serviceId);

    public async Task<IReadOnlyDictionary<Guid, ServiceCatalogEntry>> GetManyAsync(IEnumerable<Guid> serviceIds, CancellationToken cancellationToken = default)
    {
        var ids = serviceIds.Distinct().ToList();
        return await context.Set<Service>().AsNoTracking()
            .Where(s => ids.Contains(s.Id))
            .Select(s => new ServiceCatalogEntry(s.Id, s.Title, s.Kind, s.DiscountedPrice, s.TaxRate))
            .ToDictionaryAsync(s => s.Id, cancellationToken);
    }
}
