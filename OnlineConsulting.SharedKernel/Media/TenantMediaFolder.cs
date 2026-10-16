using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.SharedKernel.Media;

/// <summary>Storage folder per tenant, so each tenant's files live under one prefix.</summary>
public static class TenantMediaFolder
{
    public const string PlatformRoot = "platform";
    public const string TenantsRoot = "tenants";

    /// <summary>Root folder of a tenant's files.</summary>
    public static string Root(Guid tenantId) =>
        tenantId == TenantDefaults.DefaultTenantId ? PlatformRoot : $"{TenantsRoot}/{tenantId:D}";

    /// <summary>"{tenant root}/{folder}".</summary>
    public static string For(Guid tenantId, string folder) => $"{Root(tenantId)}/{folder}";
}
