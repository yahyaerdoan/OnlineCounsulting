namespace OnlineConsulting.SharedKernel.Tenancy;

/// <summary>Cross-module read of the time zone a tenant's business runs in; implemented by the Tenancy module.</summary>
public interface ITenantTimeZoneReader
{
    /// <summary>The tenant's zone, or the configured default for a tenant without its own (such as the default tenant).</summary>
    Task<TimeZoneInfo> GetAsync(Guid tenantId, CancellationToken cancellationToken = default);
}
