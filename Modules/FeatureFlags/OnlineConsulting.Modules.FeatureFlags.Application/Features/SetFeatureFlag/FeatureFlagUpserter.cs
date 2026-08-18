using OnlineConsulting.Modules.FeatureFlags.Application.Abstractions;
using OnlineConsulting.Modules.FeatureFlags.Application.Features.Constants;
using OnlineConsulting.Modules.FeatureFlags.Application.Features.Rules;
using OnlineConsulting.Modules.FeatureFlags.Domain;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.FeatureFlags.Application.Features.SetFeatureFlag;

/// <summary>Upsert+cache-invalidate logic shared by SetFeatureFlagCommand (writes the "current tenant", via ITenantProvider/JWT claim) and the cross-module IFeatureFlagWriter implementation in Infrastructure (writes an explicit tenant, via TenantContextOverride) - both end up at the same row, so the write path lives in exactly one place.</summary>
public class FeatureFlagUpserter(IFeatureFlagRepository repository, IFeatureFlagCacheInvalidator cacheInvalidator)
{
    public async Task<OperationResult> UpsertAsync(string key, bool isEnabled, CancellationToken cancellationToken)
    {
        if (!FeatureFlagKeys.Defaults.ContainsKey(key))
            return FeatureFlagBusinessRules.UnknownKey(key);

        var existing = await repository.GetAsync(f => f.Key == key, cancellationToken: cancellationToken);

        if (existing is null)
        {
            await repository.AddAsync(new FeatureFlag { Id = Guid.NewGuid(), Key = key, IsEnabled = isEnabled });
        }
        else
        {
            existing.IsEnabled = isEnabled;
            await repository.UpdateAsync(existing);
        }

        // Invalidates IFeatureFlagReader's own IMemoryCache (the cross-module hot-path reader, outside MediatR entirely - see FeatureFlagCache.cs). SetFeatureFlagCommand's own ICacheRemoveRequest separately clears GetFeatureFlagsQuery's CacheGroupKey via CacheRemovingBehavior; these are two independent caches serving two different call paths, not a duplicate of each other.
        cacheInvalidator.Invalidate();

        return Result.Success("Feature flag updated successfully.");
    }
}
