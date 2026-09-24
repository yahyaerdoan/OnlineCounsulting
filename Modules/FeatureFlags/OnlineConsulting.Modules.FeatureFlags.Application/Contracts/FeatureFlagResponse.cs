namespace OnlineConsulting.Modules.FeatureFlags.Application.Contracts;

/// <summary>Price/IsPurchased come from the tenant's TenantSubscriptionItem rows; null/false when unpurchased or there's no matching ModuleOffering (e.g. legacy always-on flags).</summary>
public record FeatureFlagResponse(string Key, bool IsEnabled, decimal? Price, bool IsPurchased);
