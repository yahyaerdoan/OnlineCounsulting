namespace OnlineConsulting.SharedKernel.Tenancy;

/// <summary>Ambient tenant for business code in the current request/scope - see TenantProvider (honors TenantScope, then JWT, then host) and NullTenantProvider (design-time stand-in). The data layer reads the same tenant through Core's ITenantContext.</summary>
public interface ITenantProvider
{
    Guid TenantId { get; }
}
