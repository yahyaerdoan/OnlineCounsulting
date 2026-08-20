using Microsoft.AspNetCore.Http;

namespace OnlineConsulting.SharedKernel.Tenancy;

public class TenantProvider(IHttpContextAccessor httpContextAccessor) : ITenantProvider
{
    public const string TenantClaimType = "tenant_id";

    /// <summary>Falls back to the default tenant instead of throwing when there's no claim, for genuinely anonymous requests. TenantContextOverride.BeginScope takes priority over the JWT claim - see its own doc comment.</summary>
    public Guid TenantId
    {
        get
        {
            if (TenantContextOverride.TenantId is { } overriddenTenantId)
            {
                return overriddenTenantId;
            }

            var claimValue = httpContextAccessor.HttpContext?.User.FindFirst(TenantClaimType)?.Value;

            return !string.IsNullOrEmpty(claimValue) && Guid.TryParse(claimValue, out var tenantId)
                ? tenantId
                : TenantDefaults.DefaultTenantId;
        }
    }
}
