namespace OnlineConsulting.Modules.Tenancy.Infrastructure.Branding;

/// <summary>Bound from the Tenancy:Branding config section.</summary>
public class TenantBrandingOptions
{
    /// <summary>Name shown for the default tenant: the platform's own site, signup and receipts.</summary>
    public string PlatformName { get; set; } = "ComfortPro";

    public TimeSpan CacheDuration { get; set; } = TimeSpan.FromMinutes(10);
}
