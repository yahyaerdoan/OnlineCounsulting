using Microsoft.Extensions.Caching.Memory;
using OnlineConsulting.Modules.FeatureFlags.Application;
using OnlineConsulting.Modules.FeatureFlags.Application.Features.Constants;
using OnlineConsulting.SharedKernel.FeatureFlags;
using OnlineConsulting.SharedKernel.Persistence;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.FeatureFlags.Infrastructure.Caching;

/// <summary>Read-through cache over IFeatureFlagRepository, keyed per tenant. Reads merge FeatureFlagKeys.Defaults with stored overrides once per cache miss instead of hitting the DB on every check - flags are read far more often than written. SetFeatureFlagHandler calls Invalidate() right after its own SaveChangesAsync so a toggle is visible on the very next read, the TTL below is only a safety net for cache entries this instance never got told to evict (e.g. a write from another instance in a multi-instance deployment).</summary>
public class FeatureFlagCache(IFeatureFlagRepository repository, ITenantProvider tenantProvider, IMemoryCache cache) : IFeatureFlagReader, IFeatureFlagCacheInvalidator
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    public async Task<bool> IsEnabledAsync(string key, CancellationToken cancellationToken = default)
    {
        var flags = await cache.GetOrCreateAsync(CacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = CacheDuration;
            var overrides = await repository.GetListAsync(size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);
            var overridesByKey = overrides.Items.ToDictionary(f => f.Key, f => f.IsEnabled);

            return FeatureFlagKeys.Defaults.ToDictionary(kvp => kvp.Key, kvp => overridesByKey.GetValueOrDefault(kvp.Key, kvp.Value));
        });

        return flags is not null && flags.TryGetValue(key, out var isEnabled) && isEnabled;
    }

    public void Invalidate() => cache.Remove(CacheKey);

    private string CacheKey => $"featureflags:{tenantProvider.TenantId}";
}
