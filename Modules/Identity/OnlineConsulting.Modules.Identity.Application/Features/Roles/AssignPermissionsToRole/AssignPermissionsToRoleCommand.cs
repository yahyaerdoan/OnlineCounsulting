using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
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
    [JsonIgnore]
    public string[] Roles => [RolesOperationClaims.Admin, GlobalOperationClaims.SuperAdmin, RolesOperationClaims.Update];
}

public class AssignPermissionsToRoleHandler(RoleManager<Role> roleManager, IHttpContextAccessor httpContextAccessor) : IRequestHandler<AssignPermissionsToRoleCommand, OperationResult>
{
    public async Task<OperationResult> Handle(AssignPermissionsToRoleCommand request, CancellationToken cancellationToken)
    {
        if (request.Permissions.Contains(PermissionClaimTypes.FullAccess)
            && !(httpContextAccessor.HttpContext?.User.ClaimPermissions()?.Contains(PermissionClaimTypes.FullAccess) ?? false))
            return Result.Forbidden("Only an existing full-access role holder can grant full access to another role.");

        var role = await roleManager.FindByIdAsync(request.RoleId.ToString());
        if (role is null)
            return Result.NotFound(RoleMessages.NoRoleDataFound);

        var existingPermissionClaims = (await roleManager.GetClaimsAsync(role))
            .Where(c => c.Type == PermissionClaimTypes.Type)
            .ToList();

        foreach (var claim in existingPermissionClaims.Where(c => !request.Permissions.Contains(c.Value)))
        {
            var removeResult = await roleManager.RemoveClaimAsync(role, claim);
            if (!removeResult.Succeeded)
                return Result.BadRequest($"{string.Join("; ", removeResult.Errors.Select(e => e.Description))} errors occurred while updating permissions.");
        }

        foreach (var permission in request.Permissions.Where(p => existingPermissionClaims.TrueForAll(c => c.Value != p)))
        {
            var addResult = await roleManager.AddClaimAsync(role, new Claim(PermissionClaimTypes.Type, permission));
            if (!addResult.Succeeded)
                return Result.BadRequest($"{string.Join("; ", addResult.Errors.Select(e => e.Description))} errors occurred while updating permissions.");
        }

        return Result.Success("Role permissions updated successfully.");
    }
}
