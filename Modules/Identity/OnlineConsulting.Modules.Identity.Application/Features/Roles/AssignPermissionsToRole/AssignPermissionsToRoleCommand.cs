using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using Core.SecurityLayer.Authorization;
using Core.SecurityLayer.Constants;
using Core.SecurityLayer.Extensions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using OnlineConsulting.Modules.Identity.Application.Features.Roles.Constants;
using OnlineConsulting.Modules.Identity.Domain;
using OnlineConsulting.SharedKernel.Authorization;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Security.Claims;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Identity.Application.Features.Roles.AssignPermissionsToRole;

public record AssignPermissionsToRoleCommand(Guid RoleId, List<string> Permissions) : IRequest<OperationResult>, ISecureAddRequest, ITransactionAddRequest
{
    // Role isn't tenant-scoped (no TenantId) - only SuperAdmin may edit a role shared across tenants.
    [JsonIgnore]
    public string[] Roles => [GlobalOperationClaims.SuperAdmin];
}

public class AssignPermissionsToRoleHandler(RoleManager<Role> roleManager, IHttpContextAccessor httpContextAccessor, IPermissionCatalog permissionCatalog) : IRequestHandler<AssignPermissionsToRoleCommand, OperationResult>
{
    public async Task<OperationResult> Handle(AssignPermissionsToRoleCommand request, CancellationToken cancellationToken)
    {
        if (request.Permissions.Contains(PermissionClaimTypes.FullAccess)
            && !(httpContextAccessor.HttpContext?.User.ClaimPermissions()?.Contains(PermissionClaimTypes.FullAccess) ?? false))
        {
            return Result.Forbidden("Only an existing full-access role holder can grant full access to another role.");
        }

        if (request.Permissions.Contains(GlobalOperationClaims.SuperAdmin)
            && !(httpContextAccessor.HttpContext?.User.ClaimRoles()?.Contains(GlobalOperationClaims.SuperAdmin) ?? false))
        {
            return Result.Forbidden("Only Super Admin can grant Super Admin access to another role.");
        }

        // SuperAdmin is a bypass sentinel (see RoleSeeder), not a catalog permission - same treatment as FullAccess.
        var unknownPermissions = request.Permissions
            .Where(p => p != PermissionClaimTypes.FullAccess && p != GlobalOperationClaims.SuperAdmin && !permissionCatalog.AllPermissions.Contains(p))
            .ToList();
        if (unknownPermissions.Count > 0)
        {
            return Result.BadRequest($"Unknown permission(s): {string.Join(", ", unknownPermissions)}.");
        }

        var role = await roleManager.FindByIdAsync(request.RoleId.ToString());
        if (role is null)
        {
            return Result.NotFound(RoleMessages.NoRoleDataFound);
        }

        var existingPermissionClaims = (await roleManager.GetClaimsAsync(role))
            .Where(c => c.Type == PermissionClaimTypes.Type)
            .ToList();

        foreach (var claim in existingPermissionClaims.Where(c => !request.Permissions.Contains(c.Value)))
        {
            var removeResult = await roleManager.RemoveClaimAsync(role, claim);
            if (!removeResult.Succeeded)
            {
                return Result.BadRequest($"{string.Join("; ", removeResult.Errors.Select(e => e.Description))} errors occurred while updating permissions.");
            }
        }

        foreach (var permission in request.Permissions.Where(p => existingPermissionClaims.TrueForAll(c => c.Value != p)))
        {
            var addResult = await roleManager.AddClaimAsync(role, new Claim(PermissionClaimTypes.Type, permission));
            if (!addResult.Succeeded)
            {
                return Result.BadRequest($"{string.Join("; ", addResult.Errors.Select(e => e.Description))} errors occurred while updating permissions.");
            }
        }

        return Result.Success("Role permissions updated successfully.");
    }
}
