using Core.PersistenceLayer.Repositories.EfRepositories;
using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Abstractions;
using OnlineConsulting.Modules.Commerce.Domain;
using OnlineConsulting.Modules.Commerce.Infrastructure.Persistence;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Commerce.Infrastructure.Repositories;

public class InvoiceSettingsRepository(CommerceDbContext context) : EfRepositoryBase<InvoiceSettings, Guid, CommerceDbContext>(context), IInvoiceSettingsRepository
{
    public Task<InvoiceSettings?> FindForTenantAsync(Guid tenantId, CancellationToken cancellationToken = default) =>
        context.InvoiceSettings
            .IgnoreQueryFilters([TenantEntityTypeBuilderExtensions.TenantFilterKey])
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.TenantId == tenantId, cancellationToken);
}
