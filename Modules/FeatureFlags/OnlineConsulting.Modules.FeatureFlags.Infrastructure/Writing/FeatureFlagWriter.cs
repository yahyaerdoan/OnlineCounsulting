using OnlineConsulting.Modules.FeatureFlags.Application.Features.SetFeatureFlag;
using OnlineConsulting.SharedKernel.FeatureFlags;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.FeatureFlags.Infrastructure.Writing;

/// <summary>Cross-module implementation of IFeatureFlagWriter. Delegates to the same FeatureFlagUpserter SetFeatureFlagCommand uses, but under a TenantContextOverride scope for the caller-specified tenantId - IFeatureFlagRepository's query filter and IFeatureFlagCacheInvalidator's cache key both resolve "the current tenant" through ITenantProvider, so scoping the override is enough to redirect both at the right tenant without duplicating either's logic.</summary>
public class FeatureFlagWriter(FeatureFlagUpserter upserter) : IFeatureFlagWriter
{
    public async Task SetAsync(Guid tenantId, string key, bool isEnabled, CancellationToken cancellationToken = default)
    {
        using var scope = TenantContextOverride.BeginScope(tenantId);

        var result = await upserter.UpsertAsync(key, isEnabled, cancellationToken);
        if (!result.IsSuccessful)
        {
            throw new InvalidOperationException($"Failed to set feature flag '{key}' for tenant {tenantId}: {result.Title}");
        }
    }
}
