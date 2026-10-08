
namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors GET /api/v1/admin/feature-flags's response shape.</summary>
public record FeatureFlagResponse(string Key, bool IsEnabled, decimal? Price, bool IsPurchased);
