using Core.PersistenceLayer.Repositories.Entities;

namespace OnlineConsulting.Modules.Tenancy.Domain;

/// <summary>A tenant's own email domain (e.g. "example.com"); emails go out from it once the email provider authenticates it.</summary>
public class TenantEmailDomain : SequentialGuidEntity
{
    private const string SenderLocalPart = "noreply";

    private TenantEmailDomain()
    {
    }

    public Guid TenantId { get; private set; }

    /// <summary>Lowercase domain, no scheme or "www".</summary>
    public string Domain { get; private set; } = string.Empty;

    /// <summary>One of <see cref="TenantEmailDomainStatuses"/>.</summary>
    public string Status { get; private set; } = TenantEmailDomainStatuses.Pending;

    public DateTimeOffset? ActivatedAt { get; private set; }

    public bool IsActive => Status == TenantEmailDomainStatuses.Active;

    /// <summary>From address used once active.</summary>
    public string SenderAddress => $"{SenderLocalPart}@{Domain}";

    /// <summary>Starts a domain awaiting its DNS records.</summary>
    public static TenantEmailDomain Register(Guid tenantId, string domain)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(domain);

        return new TenantEmailDomain { TenantId = tenantId, Domain = TenantDomain.NormalizeHost(domain) };
    }

    /// <summary>Provider authenticated the domain; emails now go out from it.</summary>
    public void Activate(DateTimeOffset now)
    {
        if (IsActive)
        {
            return;
        }

        Status = TenantEmailDomainStatuses.Active;
        ActivatedAt = now;
    }
}
