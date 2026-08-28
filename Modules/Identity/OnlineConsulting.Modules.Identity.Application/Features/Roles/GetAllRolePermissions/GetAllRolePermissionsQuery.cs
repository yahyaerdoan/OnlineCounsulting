using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.SecurityLayer.Authorization;
using Core.SecurityLayer.Constants;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.Identity.Application.Features.Auth;
using OnlineConsulting.Modules.Identity.Application.Features.Roles.Constants;
using OnlineConsulting.Modules.Identity.Application.Features.Roles.Contracts;
using OnlineConsulting.Modules.Identity.Domain;
using OnlineConsulting.SharedKernel.Authorization;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Identity.Application.Features.Roles.GetAllRolePermissions;

/// <summary>Every role's permissions in one call - backs the all-roles permission matrix page.</summary>
public record GetAllRolePermissionsQuery : IRequest<OperationDataResult<List<RolePermissionsResponse>>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [RolesOperationClaims.Admin, GlobalOperationClaims.SuperAdmin, RolesOperationClaims.Read];
}

public class GetAllRolePermissionsHandler(RoleManager<Role> roleManager, IPermissionCatalog permissionCatalog) : IRequestHandler<GetAllRolePermissionsQuery, OperationDataResult<List<RolePermissionsResponse>>>
{
    public async Task<OperationDataResult<List<RolePermissionsResponse>>> Handle(GetAllRolePermissionsQuery request, CancellationToken cancellationToken)
    {
        var roles = await roleManager.Roles.AsNoTracking().ToListAsync(cancellationToken);

        var items = new List<RolePermissionsResponse>();
        foreach (var role in roles)
        {
            var permissions = (await roleManager.GetClaimsAsync(role))
                .Where(c => c.Type == PermissionClaimTypes.Type)
                .Select(c => c.Value)
                .ToList();

            var expanded = RolePermissionResolver.ExpandForDisplay(permissions, permissionCatalog);
            items.Add(new RolePermissionsResponse { RoleId = role.Id, RoleName = role.Name ?? string.Empty, Permissions = expanded });
        }

        return Result.Success(items, "Role permissions retrieved successfully.");
    }
}
