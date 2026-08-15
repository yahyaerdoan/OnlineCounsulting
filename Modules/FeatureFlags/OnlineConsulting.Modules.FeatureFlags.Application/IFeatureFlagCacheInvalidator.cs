namespace OnlineConsulting.Modules.FeatureFlags.Application;

/// <summary>Lets SetFeatureFlagHandler evict the current tenant's cached flag set after a write, without depending on the cache implementation itself (that lives in Infrastructure, alongside SharedKernel's IFeatureFlagReader).</summary>
public interface IFeatureFlagCacheInvalidator
{
    void Invalidate();
}
