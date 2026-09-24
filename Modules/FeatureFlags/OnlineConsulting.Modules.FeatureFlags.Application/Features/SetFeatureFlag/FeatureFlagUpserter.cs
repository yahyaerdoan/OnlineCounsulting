using OnlineConsulting.Modules.FeatureFlags.Application.Abstractions;
using OnlineConsulting.Modules.FeatureFlags.Application.Features.Constants;
using OnlineConsulting.Modules.FeatureFlags.Application.Features.Rules;
using OnlineConsulting.Modules.FeatureFlags.Domain;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.FeatureFlags.Application.Features.SetFeatureFlag;

/// <summary>Shared upsert+invalidate logic for SetFeatureFlagCommand (current tenant) and IFeatureFlagWriter (explicit tenant via TenantContextOverride) - one write path for both.</summary>
public class FeatureFlagUpserter(IFeatureFlagRepository repository, IFeatureFlagCacheInvalidator cacheInvalidator)
{
    /// <summary>Creates or updates the flag override for <paramref name="key"/>, then invalidates the cross-module IFeatureFlagReader cache (separate from GetFeatureFlagsQuery's own CacheGroupKey, cleared via ICacheRemoveRequest).</summary>
    public async Task<OperationResult> UpsertAsync(string key, bool isEnabled, CancellationToken cancellationToken)
    {
        if (!FeatureFlagKeys.Defaults.ContainsKey(key))
        {
            return FeatureFlagBusinessRules.UnknownKey(key);
        }

        var existing = await repository.GetAsync(f => f.Key == key, cancellationToken: cancellationToken);

        if (existing is null)
        {
            _ = await repository.AddAsync(new FeatureFlag { Key = key, IsEnabled = isEnabled });
        }
        else
        {
            existing.IsEnabled = isEnabled;

            _ = await repository.UpdateAsync(existing);
        }

        cacheInvalidator.Invalidate();

        return Result.Success("Feature flag updated successfully.");
    }
}
