namespace OnlineConsulting.SharedKernel.FeatureFlags;

/// <summary>Cross-module write access for an explicitly-named tenant (not "the current tenant"), so background jobs and cross-tenant admin actions can toggle a flag with no ambient tenant of their own.</summary>
public interface IFeatureFlagWriter
{
    /// <summary>Throws InvalidOperationException if key is not a known FeatureFlagKeys value - callers should only pass keys they've already validated against their own catalog (e.g. Tenancy's ModuleOffering.Key).</summary>
    Task SetAsync(Guid tenantId, string key, bool isEnabled, CancellationToken cancellationToken = default);
}
