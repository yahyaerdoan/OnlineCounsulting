namespace OnlineConsulting.SharedKernel.Tenancy;

/// <summary>Ambient tenant for the current request/scope - see TenantProvider (JWT-backed, honors TenantContextOverride) and NullTenantProvider (design-time stand-in).</summary>
public interface ITenantProvider
{
    Guid TenantId { get; }
}
