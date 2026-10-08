using Core.PersistenceLayer.MultiTenancy;
using Microsoft.AspNetCore.Http;

namespace OnlineConsulting.SharedKernel.Tenancy;

/// <summary>Serves both the app's ITenantProvider and the data layer's ITenantContext from one resolution, so business code and query filters
/// always agree on the tenant.</summary>
public class TenantProvider(IHttpContextAccessor httpContextAccessor) : ITenantProvider, ITenantContext
{
    public const string TenantClaimType = "tenant_id";

    /// <summary>HttpContext.Items key holding the tenant resolved from the X-Tenant-Host header.</summary>
    public const string HostTenantItemKey = "TenantFromHost";

    /// <summary>An open TenantScope first, then the JWT claim, then the tenant resolved from the caller's host, then the default tenant.</summary>
    public Guid TenantId
    {
        get
        {
            if (TenantScope.Current is { } scopedTenantId)
            {
                return scopedTenantId;
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

    Guid? ITenantContext.TenantId => TenantId;
}
