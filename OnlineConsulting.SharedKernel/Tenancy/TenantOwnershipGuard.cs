using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.CurrentUser;

namespace OnlineConsulting.SharedKernel.Tenancy;

/// <summary>True if caller's tenant matches the target tenant, or caller is SuperAdmin.</summary>
public static class TenantOwnershipGuard
{
    public static bool CallerMayManage(Guid targetTenantId, Guid callerTenantId, ICurrentUserAccessor currentUserAccessor)
    {
        if (callerTenantId == targetTenantId)
        {
            return true;
        }

        return currentUserAccessor.IsInRole(GlobalOperationClaims.SuperAdmin);
    }
}
