using Microsoft.AspNetCore.Http;

namespace OnlineConsulting.SharedKernel.Tenancy;

public class TenantProvider(IHttpContextAccessor httpContextAccessor) : ITenantProvider
{
    public const string TenantClaimType = "tenant_id";

    public Guid TenantId
    {
        get
        {
            var claimValue = httpContextAccessor.HttpContext?.User.FindFirst(TenantClaimType)?.Value;
            if (string.IsNullOrEmpty(claimValue) || !Guid.TryParse(claimValue, out var tenantId))
                throw new InvalidOperationException(
                    "Tenant could not be resolved from the current request. Missing or invalid 'tenant_id' claim.");

            return tenantId;
        }
    }
}
