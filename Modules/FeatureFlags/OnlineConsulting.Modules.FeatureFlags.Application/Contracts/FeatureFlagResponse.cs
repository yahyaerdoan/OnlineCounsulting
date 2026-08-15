namespace OnlineConsulting.Modules.FeatureFlags.Application.Contracts;

public record FeatureFlagResponse(string Key, bool IsEnabled);
