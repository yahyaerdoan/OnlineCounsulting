namespace OnlineConsulting.SharedKernel.Tenancy;

/// <summary>Cross-module read of a tenant's brand; implemented by the Tenancy module.</summary>
public interface ITenantBrandReader
{
    /// <summary>The tenant's brand; the default tenant and unknown tenants get the platform's.</summary>
    Task<TenantBrand> GetAsync(Guid tenantId, CancellationToken cancellationToken = default);
}
