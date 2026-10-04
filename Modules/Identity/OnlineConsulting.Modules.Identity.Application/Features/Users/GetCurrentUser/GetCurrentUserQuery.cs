using Core.SecurityLayer.Authorization;
using MediatR;
using Microsoft.AspNetCore.Identity;
using OnlineConsulting.Modules.Identity.Application.Features.Auth;
using OnlineConsulting.Modules.Identity.Application.Features.Users.Constants;
using OnlineConsulting.Modules.Identity.Application.Features.Users.Contracts;
using OnlineConsulting.Modules.Identity.Domain;
using OnlineConsulting.SharedKernel.CurrentUser;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;

public record GetCurrentUserQuery : IRequest<OperationDataResult<UserResponse>>;

public class GetCurrentUserHandler(ICurrentUserAccessor currentUserAccessor, UserManager<User> userManager, RoleManager<Role> roleManager, IPermissionCatalog permissionCatalog)
    : IRequestHandler<GetCurrentUserQuery, OperationDataResult<UserResponse>>
{
    public async Task<OperationDataResult<UserResponse>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        if (!currentUserAccessor.IsAuthenticated)
        {
            return Result.Unauthorized<UserResponse>("User not found. Please log in and try again.");
        }

        var username = currentUserAccessor.UserName;
        if (string.IsNullOrEmpty(username))
        {
            return Result.NotFound<UserResponse>(UserMessages.UserNotFound);
        }

        var user = await userManager.FindByNameAsync(username);
        if (user is null)
        {
            return Result.NotFound<UserResponse>(UserMessages.UserNotFoundOrInvalidData);
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
