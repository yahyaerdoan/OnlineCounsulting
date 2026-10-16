namespace OnlineConsulting.Modules.Tenancy.Infrastructure.Domains;

/// <summary>Cloudflare for SaaS zone that fronts tenant domains; customers point a CNAME at <see cref="CnameTarget"/>.</summary>
public class CloudflareDomainOptions
{
    public string ApiUrl { get; set; } = "https://api.cloudflare.com/client/v4";

    /// <summary>API token with "SSL and Certificates: Edit" on the zone.</summary>
    public string ApiToken { get; set; } = string.Empty;

    public string ZoneId { get; set; } = string.Empty;

    /// <summary>Host in our zone that customer CNAMEs point to, e.g. "sites.example.com".</summary>
    public string CnameTarget { get; set; } = string.Empty;
}
