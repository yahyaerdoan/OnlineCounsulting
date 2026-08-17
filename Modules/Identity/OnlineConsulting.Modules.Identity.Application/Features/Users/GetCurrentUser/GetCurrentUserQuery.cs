using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using OnlineConsulting.Modules.Identity.Application.Features.Auth;
using OnlineConsulting.Modules.Identity.Application.Features.Users.Constants;
using OnlineConsulting.Modules.Identity.Application.Features.Users.Contracts;
using OnlineConsulting.Modules.Identity.Application.Features.Users.Abstractions;
using OnlineConsulting.Modules.Identity.Domain;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Security.Claims;

namespace OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;

public record GetCurrentUserQuery : IRequest<OperationDataResult<UserResponse>>;

public class GetCurrentUserHandler(IHttpContextAccessor httpContextAccessor, UserManager<User> userManager, RoleManager<Role> roleManager)
    : IRequestHandler<GetCurrentUserQuery, OperationDataResult<UserResponse>>
{
    public async Task<OperationDataResult<UserResponse>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var identity = httpContextAccessor.HttpContext?.User?.Identity;
        if (identity is not { IsAuthenticated: true })
            return Result.Unauthorized<UserResponse>("User not found. Please log in and try again.");

        var username = identity.Name ?? httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
        if (string.IsNullOrEmpty(username))
            return Result.NotFound<UserResponse>(UserMessages.UserNotFound);

        var user = await userManager.FindByNameAsync(username);
        if (user is null)
            return Result.BadRequest<UserResponse>(UserMessages.UserNotFoundOrInvalidData);

        var roles = await userManager.GetRolesAsync(user);
        var permissions = await RolePermissionResolver.ResolvePermissionsAsync(roleManager, roles);

        var response = new UserResponse
        {
            Id = user.Id,
            TenantId = user.TenantId,
            UserName = user.UserName ?? string.Empty,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email ?? string.Empty,
            ImageUrl = user.ImageUrl,
            Roles = [.. roles],
            Permissions = permissions,
        };

        return Result.Success(response, "User found.");
    }
}
