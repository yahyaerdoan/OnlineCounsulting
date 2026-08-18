namespace OnlineConsulting.SharedKernel.FeatureFlags;

/// <summary>Cross-module write access to an explicit tenant's feature flags - mirrors IFeatureFlagReader's cross-module contract shape, but for writes, and for a tenant the caller must name explicitly rather than "the current tenant". Lets code with no ambient tenant of its own (background/webhook handlers, or an application-layer command acting on a TenantId that may differ from the current request's own tenant - e.g. a SuperAdmin managing another tenant's modules) toggle a flag without referencing the FeatureFlags module's Domain/Infrastructure.</summary>
public interface IFeatureFlagWriter
{
    /// <summary>Throws InvalidOperationException if key is not a known FeatureFlagKeys value - callers should only pass keys they've already validated against their own catalog (e.g. Tenancy's ModuleOffering.Key).</summary>
    Task SetAsync(Guid tenantId, string key, bool isEnabled, CancellationToken cancellationToken = default);
}
