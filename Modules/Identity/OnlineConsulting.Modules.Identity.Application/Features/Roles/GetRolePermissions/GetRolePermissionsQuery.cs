using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.SecurityLayer.Constants;
using MediatR;
using Microsoft.AspNetCore.Identity;
using OnlineConsulting.Modules.Identity.Application.Features.Roles.Constants;
using OnlineConsulting.Modules.Identity.Domain;
using OnlineConsulting.SharedKernel.Authorization;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Identity.Application.Features.Roles.GetRolePermissions;

public record GetRolePermissionsQuery(Guid RoleId) : IRequest<OperationDataResult<List<string>>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [RolesOperationClaims.Admin, GlobalOperationClaims.SuperAdmin, RolesOperationClaims.Read];
}

public class GetRolePermissionsHandler(RoleManager<Role> roleManager) : IRequestHandler<GetRolePermissionsQuery, OperationDataResult<List<string>>>
{
    public async Task<OperationDataResult<List<string>>> Handle(GetRolePermissionsQuery request, CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByIdAsync(request.RoleId.ToString());
        if (role is null)
            return Result.NotFound<List<string>>(RoleMessages.NoRoleDataFound);

        var permissions = (await roleManager.GetClaimsAsync(role))
            .Where(c => c.Type == PermissionClaimTypes.Type)
            .Select(c => c.Value)
            .ToList();

        return Result.Success(permissions, "Role permissions retrieved successfully.");
    }
}
