namespace OnlineConsulting.SharedKernel.Tenancy;

/// <summary>Stand-in for design-time DbContext creation - no HTTP request to resolve a real tenant from, and none is needed.</summary>
public sealed class NullTenantProvider : ITenantProvider
{
    public Guid TenantId => Guid.Empty;
}
