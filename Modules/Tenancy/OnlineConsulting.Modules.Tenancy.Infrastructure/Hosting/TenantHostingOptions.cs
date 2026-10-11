namespace OnlineConsulting.Modules.Tenancy.Infrastructure.Hosting;

/// <summary>Bound from the Tenancy:Hosting config section.</summary>
public class TenantHostingOptions
{
    /// <summary>Origin of the platform's own site (the default tenant), e.g. "https://comfortpro.com"; its scheme and port are reused for tenant sites.</summary>
    public string PlatformOrigin { get; set; } = string.Empty;

    /// <summary>Origin of the Api itself (not the web frontend - different port in dev, e.g. "https://localhost:7012" vs. PlatformOrigin's 7196). Used for backend-to-backend callback URLs such as the Stripe Connect OAuth redirect_uri.</summary>
    public string ApiOrigin { get; set; } = string.Empty;

    /// <summary>Domain under which each tenant gets "{slug}.{SubdomainRoot}", e.g. "comfortpro.com", or "dev.localhost" locally.</summary>
    public string SubdomainRoot { get; set; } = string.Empty;

    public TimeSpan CacheDuration { get; set; } = TimeSpan.FromMinutes(10);
}
