using OnlineConsulting.Modules.FeatureFlags.Application.Features.SetFeatureFlag;
using OnlineConsulting.SharedKernel.FeatureFlags;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.FeatureFlags.Infrastructure.Writing;

/// <summary>Cross-module IFeatureFlagWriter; delegates to FeatureFlagUpserter under a TenantContextOverride scope so the ITenantProvider-resolved repository/cache target the caller's tenant.</summary>
public class FeatureFlagWriter(FeatureFlagUpserter upserter) : IFeatureFlagWriter
{
    /// <summary>Sets the flag for <paramref name="tenantId"/>; throws <see cref="InvalidOperationException"/> if the upsert fails (e.g. unknown key).</summary>
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
