namespace OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Abstractions;

/// <summary>Drops a tenant's cached time zone after it changes, so the next read sees the new one.</summary>
public interface ITenantTimeZoneCacheInvalidator
{
    void Invalidate(Guid tenantId);
}
