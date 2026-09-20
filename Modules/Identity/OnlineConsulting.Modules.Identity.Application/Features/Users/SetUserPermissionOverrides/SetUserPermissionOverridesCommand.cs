using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using Core.SecurityLayer.Authorization;
using Core.SecurityLayer.Constants;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using OnlineConsulting.Modules.Identity.Application.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Auth;
using OnlineConsulting.Modules.Identity.Application.Features.Users.Constants;
using OnlineConsulting.Modules.Identity.Domain;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.Tenancy;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Security.Claims;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Identity.Application.Features.Users.SetUserPermissionOverrides;

/// <summary>DeniedPermissions replaces the user's full denied set (same "resend the whole list" convention
/// as AssignPermissionsToRoleCommand) - every permission not in this list is restored to whatever the
/// user's role grants.</summary>
public record SetUserPermissionOverridesCommand(Guid UserId, List<string> DeniedPermissions) : IRequest<OperationResult>, ISecureAddRequest, ITransactionAddRequest
{
    [JsonIgnore]
    public string[] Roles => [UsersOperationClaims.Admin, GlobalOperationClaims.SuperAdmin, UsersOperationClaims.Write];
}

public class SetUserPermissionOverridesHandler(UserManager<User> userManager, RoleManager<Role> roleManager, IPermissionCatalog permissionCatalog, ITenantOwnershipReader tenantOwnershipReader, ITenantProvider tenantProvider, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<SetUserPermissionOverridesCommand, OperationResult>
{
    public async Task<OperationResult> Handle(SetUserPermissionOverridesCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString());
        if (user is null)
        {
            return Result.NotFound(UserMessages.UserNotFound);
        }

        var ownerGuardResult = await TenantOwnerProtection.EnsureCallerMayModifyAsync(userManager, tenantOwnershipReader, tenantProvider, httpContextAccessor, user, cancellationToken);
        if (ownerGuardResult is not null)
        {
            return ownerGuardResult;
        }

        // Only permissions the user's own role actually grants can be denied - a bypass claim
        // (FullAccess/TenantFullAccess/SuperAdmin) or a permission the role never had isn't a valid target.
        var roles = await userManager.GetRolesAsync(user);
        var rolePermissions = await RolePermissionResolver.ResolvePermissionsAsync(roleManager, roles);

        var invalidPermissions = request.DeniedPermissions.Where(p => !rolePermissions.Contains(p) || !permissionCatalog.AllPermissions.Contains(p)).ToList();
        if (invalidPermissions.Count > 0)
        {
            return Result.BadRequest($"Cannot deny permission(s) the user's role doesn't grant: {string.Join(", ", invalidPermissions)}.");
        }

        var existingDeniedClaims = (await userManager.GetClaimsAsync(user)).Where(c => c.Type == PermissionOverrideClaimTypes.Deny).ToList();

        foreach (var claim in existingDeniedClaims.Where(c => !request.DeniedPermissions.Contains(c.Value)))
        {
            var removeResult = await userManager.RemoveClaimAsync(user, claim);
            if (!removeResult.Succeeded)
            {
                return Result.BadRequest($"{string.Join("; ", removeResult.Errors.Select(e => e.Description))} errors occurred while updating permission overrides.");
            }
        }

        foreach (var permission in request.DeniedPermissions.Where(p => existingDeniedClaims.TrueForAll(c => c.Value != p)))
        {
            var addResult = await userManager.AddClaimAsync(user, new Claim(PermissionOverrideClaimTypes.Deny, permission));
            if (!addResult.Succeeded)
            {
                return Result.BadRequest($"{string.Join("; ", addResult.Errors.Select(e => e.Description))} errors occurred while updating permission overrides.");
            }
        }

        return Result.Success("User permission overrides updated successfully.");
    }
}
