namespace OnlineConsulting.Modules.Tenancy.Domain;

/// <summary>Values of <see cref="TenantEmailDomain.Status"/>.</summary>
public static class TenantEmailDomainStatuses
{
    /// <summary>Waiting for the tenant's DNS records.</summary>
    public const string Pending = "Pending";

    /// <summary>Emails go out from this domain.</summary>
    public const string Active = "Active";
}
