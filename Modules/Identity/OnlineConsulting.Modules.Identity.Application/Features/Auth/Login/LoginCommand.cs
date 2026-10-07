using MediatR;
using Microsoft.AspNetCore.Identity;
using OnlineConsulting.Modules.Identity.Application.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Auth.Abstractions;
using OnlineConsulting.Modules.Identity.Application.Features.Auth.Constants;
using OnlineConsulting.Modules.Identity.Application.Features.Auth.Contracts;
using OnlineConsulting.Modules.Identity.Domain;
using OnlineConsulting.SharedKernel.Tenancy;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Identity.Application.Features.Auth.Login;

public record LoginCommand(string UserNameOrEmail, string Password) : IRequest<OperationDataResult<AuthTokensResponse>>;

public class LoginHandler(UserManager<User> userManager, RoleManager<Role> roleManager, IPasswordChecker passwordChecker, ITokenService tokenService, IRefreshTokenService refreshTokenService, ITenantProvider tenantProvider)
    : IRequestHandler<LoginCommand, OperationDataResult<AuthTokensResponse>>
{
    public async Task<OperationDataResult<AuthTokensResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var tenantId = tenantProvider.TenantId;
        var user = await userManager.FindByNameAsync(request.UserNameOrEmail) is { } byUserName && byUserName.TenantId == tenantId
            ? byUserName
            : await userManager.FindByEmailInTenantAsync(request.UserNameOrEmail, tenantId, cancellationToken);

        if (user is null)
        {
            return Result.BadRequest<AuthTokensResponse>(AuthMessages.InvalidCredentials);
        }

        var passwordCheck = await passwordChecker.CheckAsync(user, request.Password);

        if (passwordCheck.IsLockedOut)
        {
            return Result.Forbidden<AuthTokensResponse>(AuthMessages.AccountLocked);
        }

        if (!passwordCheck.Succeeded)
        {
            return Result.BadRequest<AuthTokensResponse>(AuthMessages.InvalidCredentials);
        }

        if (!await userManager.IsEmailConfirmedAsync(user))
        {
            return Result.BadRequest<AuthTokensResponse>(AuthMessages.EmailNotConfirmed);
        }

        var roles = await userManager.GetRolesAsync(user);
        var rolePermissions = await RolePermissionResolver.ResolvePermissionsAsync(roleManager, roles);
        var permissions = await RolePermissionResolver.ApplyUserOverridesAsync(userManager, user, rolePermissions);

        var (accessToken, accessTokenExpiresAt) = tokenService.CreateAccessToken(user, [.. roles], permissions);

        var (refreshToken, _) = await refreshTokenService.IssueAsync(user, cancellationToken);

        return Result.Success(new AuthTokensResponse(user.Id, accessToken, refreshToken, accessTokenExpiresAt), "Credentials validated successfully.");
    }
}
