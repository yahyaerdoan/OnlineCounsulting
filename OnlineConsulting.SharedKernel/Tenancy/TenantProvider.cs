using Microsoft.AspNetCore.Http;

namespace OnlineConsulting.SharedKernel.Tenancy;

public class TenantProvider(IHttpContextAccessor httpContextAccessor) : ITenantProvider
{
    public const string TenantClaimType = "tenant_id";

    // Falls back to the default tenant instead of throwing when there's no claim - needed for
    // genuinely anonymous requests (e.g. a guest shopping basket, see IGuestIdAccessor) that never
    // carry a JWT at all. Every authenticated endpoint already requires the claim via
    // RequireAuthorization()/ISecureAddRequest, so this only ever applies to callers that were
    // never expected to have one. Revisit once real multi-tenant onboarding exists and anonymous
    // callers need a way to signal which tenant they're shopping under (e.g. by subdomain/host).
    public Guid TenantId
    {
        get
        {
            var claimValue = httpContextAccessor.HttpContext?.User.FindFirst(TenantClaimType)?.Value;

            return !string.IsNullOrEmpty(claimValue) && Guid.TryParse(claimValue, out var tenantId)
                ? tenantId
                : TenantDefaults.DefaultTenantId;
        }
    }
}
