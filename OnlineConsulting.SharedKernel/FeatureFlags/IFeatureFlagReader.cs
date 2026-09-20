namespace OnlineConsulting.SharedKernel.FeatureFlags;

/// <summary>Cross-module read access to the current tenant's feature flags, so any module can gate behavior without referencing the FeatureFlags module's Domain/Infrastructure.</summary>
public interface IFeatureFlagReader
{
    Task<bool> IsEnabledAsync(string key, CancellationToken cancellationToken = default);
}
