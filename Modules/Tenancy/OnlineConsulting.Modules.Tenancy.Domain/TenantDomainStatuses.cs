namespace OnlineConsulting.Modules.Tenancy.Domain;

/// <summary>Values of <see cref="TenantDomain.Status"/>.</summary>
public static class TenantDomainStatuses
{
    /// <summary>Waiting for the tenant's DNS records and the certificate.</summary>
    public const string Pending = "Pending";

    /// <summary>Serving the tenant's site.</summary>
    public const string Active = "Active";
}
