using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.SecurityLayer.Authorization;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using OnlineConsulting.Modules.Identity.Application.Features.Auth;
using OnlineConsulting.Modules.Identity.Application.Features.Users.Constants;
using OnlineConsulting.Modules.Identity.Application.Features.Users.Contracts;
using OnlineConsulting.Modules.Identity.Domain;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.Tenancy;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Identity.Application.Features.Users.GetUserPermissionOverrides;

public record GetUserPermissionOverridesQuery(Guid UserId) : IRequest<OperationDataResult<UserPermissionOverridesResponse>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [UsersOperationClaims.Admin, GlobalOperationClaims.SuperAdmin, UsersOperationClaims.Read];
}

public class GetUserPermissionOverridesHandler(UserManager<User> userManager, RoleManager<Role> roleManager, IPermissionCatalog permissionCatalog, ITenantProvider tenantProvider, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<GetUserPermissionOverridesQuery, OperationDataResult<UserPermissionOverridesResponse>>
{
    public async Task<OperationDataResult<UserPermissionOverridesResponse>> Handle(GetUserPermissionOverridesQuery request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString());
        if (user is null)
        {
            return Result.NotFound<UserPermissionOverridesResponse>(UserMessages.UserNotFound);
        }

        if (!TenantOwnershipGuard.CallerMayManage(user.TenantId, tenantProvider.TenantId, httpContextAccessor))
        {
            return Result.Forbidden<UserPermissionOverridesResponse>(UserMessages.NotAuthorizedForOtherTenant);
        }

        var roles = await userManager.GetRolesAsync(user);
        var rolePermissions = await RolePermissionResolver.ResolvePermissionsAsync(roleManager, roles);

        // A bypass-holding user (FullAccess/TenantFullAccess/SuperAdmin) has no meaningful catalog baseline
        // to narrow - restrict the editable list to real catalog permissions only.
        var editablePermissions = rolePermissions.Where(permissionCatalog.AllPermissions.Contains).ToList();

        var deniedPermissions = (await userManager.GetClaimsAsync(user)).Where(c => c.Type == PermissionOverrideClaimTypes.Deny).Select(c => c.Value).ToList();

        return Result.Success(new UserPermissionOverridesResponse(editablePermissions, deniedPermissions), "User permission overrides retrieved successfully.");
    }
}
