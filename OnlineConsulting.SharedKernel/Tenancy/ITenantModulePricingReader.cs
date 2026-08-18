namespace OnlineConsulting.SharedKernel.Tenancy;

/// <summary>Cross-module read access to a tenant's purchased-module pricing - lets GetFeatureFlagsQuery (FeatureFlags module) show Price/IsPurchased per flag key without referencing the Tenancy module's Domain/Application types, matching the project's cross-module convention (plain ids/shared-kernel interfaces only, see IFeatureFlagReader/IFeatureFlagWriter/ITenantStatusReader for the same pattern).</summary>
public interface ITenantModulePricingReader
{
    /// <summary>Keyed by ModuleOffering.Key / FeatureFlagKeys value. Only contains keys the tenant has an active (non-removed) TenantSubscriptionItem for - callers should treat a missing key as "not purchased".</summary>
    Task<IReadOnlyDictionary<string, (decimal Price, bool IsPurchased)>> GetForTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
}
