using Core.SecurityLayer.Extensions;
using Microsoft.AspNetCore.Http;
using OnlineConsulting.Modules.Identity.Application.Features.Users.Rules;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.Tenancy;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Identity.Application.Common;

/// <summary>Guards a write to another user: same tenant required, target can't be the owner. Null = allowed.</summary>
public static class TenantOwnerProtection
{
    public static async Task<OperationResult?> EnsureCallerMayModifyAsync(ITenantOwnershipReader tenantOwnershipReader, ITenantProvider tenantProvider, IHttpContextAccessor httpContextAccessor, Guid tenantId, Guid targetUserId, CancellationToken cancellationToken = default)
    {
        if (!TenantOwnershipGuard.CallerMayManage(tenantId, tenantProvider.TenantId, httpContextAccessor))
        {
            return UserBusinessRules.NotAuthorizedForOtherTenant();
        }

        var callerRoles = httpContextAccessor.HttpContext?.User.ClaimRoles() ?? [];
        if (callerRoles.Contains(GlobalOperationClaims.SuperAdmin))
        {
            return null;
        }

        var targetIsOwner = await tenantOwnershipReader.IsOwnerAsync(tenantId, targetUserId, cancellationToken);
        return targetIsOwner
            ? Result.Forbidden("The tenant owner's role or membership cannot be changed by another admin. Only a Super Admin may do this.")
            : null;
    }
}
