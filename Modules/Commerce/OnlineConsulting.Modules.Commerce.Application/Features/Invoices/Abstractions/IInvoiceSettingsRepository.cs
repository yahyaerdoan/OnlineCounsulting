using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.Commerce.Domain;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Abstractions;

public interface IInvoiceSettingsRepository : IAsyncRepository<InvoiceSettings, Guid>
{
    /// <summary>The given tenant's settings regardless of the ambient tenant, for invoices issued from webhooks and background jobs.</summary>
    Task<InvoiceSettings?> FindForTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
}
