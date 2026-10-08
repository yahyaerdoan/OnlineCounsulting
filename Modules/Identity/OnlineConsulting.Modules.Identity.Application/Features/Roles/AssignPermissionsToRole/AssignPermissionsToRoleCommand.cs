using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.SecurityLayer.Authorization;
using Core.SecurityLayer.Constants;
using MediatR;
using Microsoft.AspNetCore.Identity;
using OnlineConsulting.Modules.Identity.Application.Features.Roles.Constants;
using OnlineConsulting.Modules.Identity.Domain;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.CurrentUser;
using OnlineConsulting.SharedKernel.Transactions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Security.Claims;

namespace OnlineConsulting.Modules.Identity.Application.Features.Roles.AssignPermissionsToRole;

public record AssignPermissionsToRoleCommand(Guid RoleId, List<string> Permissions) : IRequest<OperationResult>, ISecureAddRequest, IIdentityTransactionRequest
{
    /// <summary>Roles aren't tenant-scoped, so only Super Admin may edit one - it's shared across tenants.</summary>
    public string[] Roles => [GlobalOperationClaims.SuperAdmin];

    /// <summary>Cross-tenant/platform-level - a tenant admin must never reach this, even with TenantFullAccess.</summary>
    public bool AllowTenantBypass => false;
}

/// <summary>
/// Replaces a role's permission claims with the given set. Granting FullAccess or Super Admin requires the
/// caller already hold that same privilege. SuperAdmin is a bypass sentinel (see RoleBootstrapper), not a catalog
/// permission, so it is validated the same way as FullAccess rather than checked against the catalog.
/// </summary>
public class AssignPermissionsToRoleHandler(RoleManager<Role> roleManager, ICurrentUserAccessor currentUserAccessor, IPermissionCatalog permissionCatalog) : IRequestHandler<AssignPermissionsToRoleCommand, OperationResult>
{
    public async Task<OperationResult> Handle(AssignPermissionsToRoleCommand request, CancellationToken cancellationToken)
    {
        if (request.Permissions.Contains(PermissionClaimTypes.FullAccess) && !currentUserAccessor.HasPermission(PermissionClaimTypes.FullAccess))
        {
            return Result.Forbidden("Only an existing full-access role holder can grant full access to another role.");
        }

        if (request.Permissions.Contains(GlobalOperationClaims.SuperAdmin) && !currentUserAccessor.IsInRole(GlobalOperationClaims.SuperAdmin))
        {
            return Result.Forbidden("Only Super Admin can grant Super Admin access to another role.");
        }

        var unknownPermissions = request.Permissions.Where(p => p != PermissionClaimTypes.FullAccess && p != GlobalOperationClaims.SuperAdmin && !permissionCatalog.AllPermissions.Contains(p)).ToList();

        if (unknownPermissions.Count > 0)
        {
            return Result.UnprocessableContent($"Unknown permission(s): {string.Join(", ", unknownPermissions)}.");
        }

        var role = await roleManager.FindByIdAsync(request.RoleId.ToString());

        if (role is null)
        {
            return Result.NotFound(RoleMessages.NoRoleDataFound);
        }

        var existingPermissionClaims = (await roleManager.GetClaimsAsync(role)).Where(c => c.Type == PermissionClaimTypes.Type).ToList();

        foreach (var claim in existingPermissionClaims.Where(c => !request.Permissions.Contains(c.Value)))
        {
            var removeResult = await roleManager.RemoveClaimAsync(role, claim);
            if (!removeResult.Succeeded)
            {
                return Result.InternalServerError($"{string.Join("; ", removeResult.Errors.Select(e => e.Description))} errors occurred while updating permissions.");
            }
        }

        foreach (var permission in request.Permissions.Where(p => existingPermissionClaims.TrueForAll(c => c.Value != p)))
        {
            var addResult = await roleManager.AddClaimAsync(role, new Claim(PermissionClaimTypes.Type, permission));
            if (!addResult.Succeeded)
            {
                return Result.InternalServerError($"{string.Join("; ", addResult.Errors.Select(e => e.Description))} errors occurred while updating permissions.");
            }
        }

        return Result.Success("Role permissions updated successfully.");
    }
}
