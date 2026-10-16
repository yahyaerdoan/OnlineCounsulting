using Core.PersistenceLayer.Repositories.Entities;

namespace OnlineConsulting.Modules.Tenancy.Domain;

/// <summary>A tenant's own domain (e.g. "www.example.com"); serves the tenant's site once DNS and the certificate are in place.</summary>
public class TenantDomain : SequentialGuidEntity
{
    private TenantDomain()
    {
    }

    public Guid TenantId { get; private set; }

    /// <summary>Lowercase host, no scheme or port.</summary>
    public string Host { get; private set; } = string.Empty;

    /// <summary>Hosting provider's id for the domain.</summary>
    public string ProviderDomainId { get; private set; } = string.Empty;

    /// <summary>One of <see cref="TenantDomainStatuses"/>.</summary>
    public string Status { get; private set; } = TenantDomainStatuses.Pending;

    public DateTimeOffset? ActivatedAt { get; private set; }

    public bool IsActive => Status == TenantDomainStatuses.Active;

    /// <summary>Starts a domain awaiting DNS and its certificate.</summary>
    public static TenantDomain Register(Guid tenantId, string host, string providerDomainId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(host);
        ArgumentException.ThrowIfNullOrWhiteSpace(providerDomainId);

        return new TenantDomain { TenantId = tenantId, Host = NormalizeHost(host), ProviderDomainId = providerDomainId };
    }

    /// <summary>Trimmed, lowercase, no trailing dot.</summary>
    public static string NormalizeHost(string host) => host.Trim().TrimEnd('.').ToLowerInvariant();

    /// <summary>DNS and certificate are ready; starts serving the site.</summary>
    public void Activate(DateTimeOffset now)
    {
        if (IsActive)
        {
            return;
        }

        Status = TenantDomainStatuses.Active;
        ActivatedAt = now;
    }
}
