using Core.SecurityLayer.Extensions;
using Microsoft.AspNetCore.Http;
using OnlineConsulting.SharedKernel.Authorization;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptionItems;

/// <summary>AddModuleCommand/RemoveModuleCommand declare no Roles (ISecureAddRequest.Roles => [], any authenticated caller passes the pipeline's declarative gate) because "who may call this" isn't a role, it's "the caller's own tenant, or a platform SuperAdmin acting on someone else's" - a per-request ownership check the declarative Roles array has no way to express. This is the one place in the codebase doing that combination, so it's factored out instead of duplicated between the two handlers.</summary>
public static class TenantOwnershipGuard
{
    public static bool CallerMayManage(Guid targetTenantId, Guid callerTenantId, IHttpContextAccessor httpContextAccessor)
    {
        if (callerTenantId == targetTenantId)
            return true;

        var roles = httpContextAccessor.HttpContext?.User.ClaimRoles() ?? [];
        return roles.Contains(GlobalOperationClaims.SuperAdmin);
    }
}
