using Core.SecurityLayer.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using OnlineConsulting.Modules.Identity.Application.Features.Users.Rules;
using OnlineConsulting.Modules.Identity.Domain;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.Tenancy;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Identity.Application.Common;

/// <summary>Guards a write to another user: same tenant required, target can't be a Super Admin or the tenant owner. Null = allowed.</summary>
public static class TenantOwnerProtection
{
    public static async Task<OperationResult?> EnsureCallerMayModifyAsync(UserManager<User> userManager, ITenantOwnershipReader tenantOwnershipReader, ITenantProvider tenantProvider, IHttpContextAccessor httpContextAccessor, User target, CancellationToken cancellationToken = default)
    {
        if (!TenantOwnershipGuard.CallerMayManage(target.TenantId, tenantProvider.TenantId, httpContextAccessor))
        {
            return UserBusinessRules.NotAuthorizedForOtherTenant();
        }

        var callerRoles = httpContextAccessor.HttpContext?.User.ClaimRoles() ?? [];
        if (callerRoles.Contains(GlobalOperationClaims.SuperAdmin))
        {
            return null;
        }

        // Target's own role, not the caller's - a non-SuperAdmin can otherwise land in the same
        // TenantId as a SuperAdmin (e.g. invited directly by one) and would pass every other check.
        if (await userManager.IsInRoleAsync(target, GlobalOperationClaims.SuperAdmin))
        {
            return Result.Forbidden("A Super Admin's role or membership cannot be changed by another admin.");
        }

        var targetIsOwner = await tenantOwnershipReader.IsOwnerAsync(target.TenantId, target.Id, cancellationToken);
        return targetIsOwner
            ? Result.Forbidden("The tenant owner's role or membership cannot be changed by another admin. Only a Super Admin may do this.")
            : null;
    }
}
