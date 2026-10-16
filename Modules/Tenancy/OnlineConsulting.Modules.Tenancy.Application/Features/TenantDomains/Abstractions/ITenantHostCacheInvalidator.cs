namespace OnlineConsulting.Modules.Tenancy.Application.Features.TenantDomains.Abstractions;

/// <summary>Drops cached host and origin lookups after a tenant's domain changes.</summary>
public interface ITenantHostCacheInvalidator
{
    void Invalidate(Guid tenantId, string host);
}
