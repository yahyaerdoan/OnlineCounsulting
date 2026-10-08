namespace OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Abstractions;

/// <summary>Drops a tenant's cached brand after it changes, so the next read sees the new one.</summary>
public interface ITenantBrandCacheInvalidator
{
    void Invalidate(Guid tenantId);
}
