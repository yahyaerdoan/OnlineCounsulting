using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Abstractions;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Tenancy.Infrastructure.Contacts;

/// <summary>Cross-module implementation of ITenantContactReader.</summary>
public class TenantContactReader(ITenantRepository tenantRepository) : ITenantContactReader
{
    public async Task<string?> GetContactEmailAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var tenant = await tenantRepository.GetAsync(t => t.Id == tenantId, enableTracking: false, cancellationToken: cancellationToken);
        return tenant?.PrimaryContactEmail;
    }
}
