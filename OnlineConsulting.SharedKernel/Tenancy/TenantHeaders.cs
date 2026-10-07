namespace OnlineConsulting.SharedKernel.Tenancy;

public static class TenantHeaders
{
    /// <summary>Host of the site or app the caller belongs to; picks the tenant for requests without a tenant claim.</summary>
    public const string Host = "X-Tenant-Host";
}
