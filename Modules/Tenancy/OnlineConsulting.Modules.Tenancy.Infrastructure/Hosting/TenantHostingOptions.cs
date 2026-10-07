namespace OnlineConsulting.Modules.Tenancy.Infrastructure.Hosting;

/// <summary>Bound from the Tenancy:Hosting config section.</summary>
public class TenantHostingOptions
{
    /// <summary>Origin of the platform's own site (the default tenant), e.g. "https://comfortpro.com"; its scheme and port are reused for tenant sites.</summary>
    public string PlatformOrigin { get; set; } = string.Empty;

    /// <summary>Domain under which each tenant gets "{slug}.{SubdomainRoot}", e.g. "comfortpro.com", or "dev.localhost" locally.</summary>
    public string SubdomainRoot { get; set; } = string.Empty;

    public TimeSpan CacheDuration { get; set; } = TimeSpan.FromMinutes(10);
}
