namespace OnlineConsulting.Modules.Tenancy.Infrastructure.Domains;

/// <summary>Bound from the Tenancy:CustomDomains config section.</summary>
public class TenantDomainOptions
{
    /// <summary>One of <see cref="CustomDomainProviderNames"/>.</summary>
    public string Provider { get; set; } = CustomDomainProviderNames.Mock;

    /// <summary>How often pending domains are checked.</summary>
    public TimeSpan PollInterval { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>A domain still pending after this is removed, freeing the provider slot.</summary>
    public TimeSpan PendingTimeout { get; set; } = TimeSpan.FromDays(7);

    public RailwayDomainOptions Railway { get; set; } = new();

    public CloudflareDomainOptions Cloudflare { get; set; } = new();
}
