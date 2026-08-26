using Core.SecurityLayer.Extensions;
using Microsoft.AspNetCore.Http;
using OnlineConsulting.SharedKernel.Authorization;

namespace OnlineConsulting.SharedKernel.Tenancy;

/// <summary>True if caller's tenant matches the target tenant, or caller is SuperAdmin.</summary>
public static class TenantOwnershipGuard
{
    public static bool CallerMayManage(Guid targetTenantId, Guid callerTenantId, IHttpContextAccessor httpContextAccessor)
    {
        if (callerTenantId == targetTenantId)
        {
            return true;
        }

        var roles = httpContextAccessor.HttpContext?.User.ClaimRoles() ?? [];
        return roles.Contains(GlobalOperationClaims.SuperAdmin);
    }
}
