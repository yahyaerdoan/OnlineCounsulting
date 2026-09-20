namespace OnlineConsulting.SharedKernel.Tenancy;

/// <summary>Cross-module read access to a tenant's purchased-module pricing, so GetFeatureFlagsQuery can show Price/IsPurchased per flag key without referencing Tenancy's Domain/Application types.</summary>
public interface ITenantModulePricingReader
{
    /// <summary>Keyed by ModuleOffering.Key / FeatureFlagKeys value. Only contains keys the tenant has an active (non-removed) TenantSubscriptionItem for - callers should treat a missing key as "not purchased".</summary>
    Task<IReadOnlyDictionary<string, (decimal Price, bool IsPurchased)>> GetForTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
}
