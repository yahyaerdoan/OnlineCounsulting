using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Tenancy.Infrastructure.TimeZones;

/// <summary>Bound from the Tenancy:TimeZone config section.</summary>
public class TenantTimeZoneOptions
{
    /// <summary>Zone for tenants without their own row, such as the default tenant behind the public site.</summary>
    public string DefaultTimeZoneId { get; set; } = BusinessTimeZones.Default;

    public TimeSpan CacheDuration { get; set; } = TimeSpan.FromMinutes(10);
}
