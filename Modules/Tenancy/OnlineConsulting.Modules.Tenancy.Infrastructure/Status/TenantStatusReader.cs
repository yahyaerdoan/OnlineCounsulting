using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Abstractions;
using OnlineConsulting.Modules.Tenancy.Domain;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Tenancy.Infrastructure.Status;

/// <summary>Cross-module implementation of ITenantStatusReader, backing TenantStatusCheckBehavior in SharedKernel.</summary>
public class TenantStatusReader(ITenantRepository tenantRepository) : ITenantStatusReader
{
    public async Task<bool> IsSuspendedAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var tenant = await tenantRepository.GetAsync(t => t.Id == tenantId, cancellationToken: cancellationToken);
        return tenant?.Status == TenantStatuses.Suspended;
    }
}
