using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.FeatureFlags.Domain;

/// <summary>Only stores rows that override a coded default (see FeatureFlagKeys.Defaults) - absence of a row for a given (TenantId, Key) means "use the coded default".</summary>
public class FeatureFlag : TenantEntity<Guid>
{
    public required string Key { get; set; }
    public required bool IsEnabled { get; set; }
}
