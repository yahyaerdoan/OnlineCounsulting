using Core.PersistenceLayer.MultiTenancy;

namespace OnlineConsulting.SharedKernel.Tenancy;

/// <summary>Stand-in for design-time DbContext creation - no HTTP request to resolve a real tenant from, and none is needed.</summary>
public sealed class NullTenantProvider : ITenantProvider, ITenantContext
{
    public Guid TenantId => Guid.Empty;

    Guid? ITenantContext.TenantId => null;
}
