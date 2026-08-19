using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Abstractions;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Tenancy.Infrastructure.Status;

/// <summary>Cross-module implementation of ITenantOwnershipReader, backing TenantOwnerProtection in the Identity module.</summary>
public class TenantOwnershipReader(ITenantRepository tenantRepository) : ITenantOwnershipReader
{
    public async Task<bool> IsOwnerAsync(Guid tenantId, Guid userId, CancellationToken cancellationToken = default)
    {
        var tenant = await tenantRepository.GetAsync(t => t.Id == tenantId, cancellationToken: cancellationToken);
        return tenant?.OwnerUserId == userId;
    }
}
