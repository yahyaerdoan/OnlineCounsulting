using Microsoft.AspNetCore.Http;

namespace OnlineConsulting.SharedKernel.Tenancy;

public class TenantProvider(IHttpContextAccessor httpContextAccessor) : ITenantProvider
{
    public const string TenantClaimType = "tenant_id";

    /// <summary>HttpContext.Items key holding the tenant resolved from the X-Tenant-Host header.</summary>
    public const string HostTenantItemKey = "TenantFromHost";

    /// <summary>TenantContextOverride.BeginScope first, then the JWT claim, then the tenant resolved from the caller's host, then the default tenant.</summary>
    public Guid TenantId
    {
        get
        {
            if (TenantContextOverride.TenantId is { } overriddenTenantId)
            {
                return overriddenTenantId;
            }

            var httpContext = httpContextAccessor.HttpContext;
            var claimValue = httpContext?.User.FindFirst(TenantClaimType)?.Value;

            if (!string.IsNullOrEmpty(claimValue) && Guid.TryParse(claimValue, out var tenantId))
            {
                return tenantId;
            }

            return httpContext?.Items[HostTenantItemKey] is Guid hostTenantId
                ? hostTenantId
                : TenantDefaults.DefaultTenantId;
        }
    }
}
