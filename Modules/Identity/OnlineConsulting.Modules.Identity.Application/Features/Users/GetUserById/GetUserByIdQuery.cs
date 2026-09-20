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

namespace OnlineConsulting.Modules.Identity.Application.Features.Users.GetUserById;

public record GetUserByIdQuery(Guid UserId) : IRequest<OperationDataResult<UserResponse>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [UsersOperationClaims.Admin, GlobalOperationClaims.SuperAdmin, UsersOperationClaims.Read];
}

public class GetUserByIdHandler(UserManager<User> userManager, RoleManager<Role> roleManager, IPermissionCatalog permissionCatalog, ITenantProvider tenantProvider, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<GetUserByIdQuery, OperationDataResult<UserResponse>>
{
    public async Task<OperationDataResult<UserResponse>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString());
        if (user is null)
        {
            return Result.NotFound<UserResponse>(UserMessages.UserNotFound);
        }

        if (!TenantOwnershipGuard.CallerMayManage(user.TenantId, tenantProvider.TenantId, httpContextAccessor))
        {
            return Result.Forbidden<UserResponse>(UserMessages.NotAuthorizedForOtherTenant);
        }

        var roles = await userManager.GetRolesAsync(user);
        var permissions = RolePermissionResolver.ExpandForDisplay(await RolePermissionResolver.ResolvePermissionsAsync(roleManager, roles), permissionCatalog);

        var response = new UserResponse
        {
            Id = user.Id,
            TenantId = user.TenantId,
            UserName = user.UserName ?? string.Empty,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email ?? string.Empty,
            ImageUrl = user.ImageUrl,
            IsActive = user.IsActive,
            Roles = [.. roles],
            Permissions = permissions,
        };

        return Result.Success(response, "User found.");
    }
}
