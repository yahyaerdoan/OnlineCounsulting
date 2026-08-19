using Core.SecurityLayer.Extensions;
using Microsoft.AspNetCore.Http;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.Tenancy;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Identity.Application.Common;

/// <summary>Call this from any command that changes an existing user's tenant role (e.g.
/// AssignRoleToUserCommand) or removes a user from a tenant (e.g. DeleteUserCommand), right after the target
/// user and their TenantId are known. Rejects the operation with a clear message when the target user is that
/// tenant's owner (Tenant.OwnerUserId) and the caller is not a platform SuperAdmin - an Admin-role teammate
/// invited later into the same tenant must never be able to demote or remove the tenant's actual owner.
/// Returns null when the operation may proceed.</summary>
public static class TenantOwnerProtection
{
    public static async Task<OperationResult?> EnsureCallerMayModifyAsync(ITenantOwnershipReader tenantOwnershipReader, IHttpContextAccessor httpContextAccessor, Guid tenantId, Guid targetUserId, CancellationToken cancellationToken = default)
    {
        var callerRoles = httpContextAccessor.HttpContext?.User.ClaimRoles() ?? [];
        if (callerRoles.Contains(GlobalOperationClaims.SuperAdmin))
            return null;

        var targetIsOwner = await tenantOwnershipReader.IsOwnerAsync(tenantId, targetUserId, cancellationToken);
        return targetIsOwner
            ? Result.Forbidden("The tenant owner's role or membership cannot be changed by another admin. Only a Super Admin may do this.")
            : null;
    }
}
