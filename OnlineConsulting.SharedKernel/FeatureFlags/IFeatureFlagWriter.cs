namespace OnlineConsulting.SharedKernel.FeatureFlags;

/// <summary>Cross-module write access for an explicitly-named tenant (not "the current tenant"), so background jobs and cross-tenant admin actions can toggle a flag with no ambient tenant of their own.</summary>
public interface IFeatureFlagWriter
{
    /// <summary>Sets a module's flag for the tenant; a key with no flag (an ungated module) is a no-op. Throws InvalidOperationException if the write fails.</summary>
    Task SetAsync(Guid tenantId, string key, bool isEnabled, CancellationToken cancellationToken = default);
}
