namespace OnlineConsulting.Modules.FeatureFlags.Application.Contracts;

/// <summary>Price/IsPurchased come from the tenant's TenantSubscriptionItem rows (via ITenantModulePricingReader) - null/false for keys the tenant hasn't purchased, or that have no matching ModuleOffering at all (e.g. legacy always-on flags).</summary>
public record FeatureFlagResponse(string Key, bool IsEnabled, decimal? Price, bool IsPurchased);
