namespace OnlineConsulting.SharedKernel.FeatureFlags;

/// <summary>Cross-module read access to the current tenant's feature flags - lets any module gate behavior on a flag (e.g. return 404 when a feature is off) without referencing the FeatureFlags module's Domain/Infrastructure, matching the project's cross-module convention (plain ids/shared-kernel interfaces only).</summary>
public interface IFeatureFlagReader
{
    Task<bool> IsEnabledAsync(string key, CancellationToken cancellationToken = default);
}
