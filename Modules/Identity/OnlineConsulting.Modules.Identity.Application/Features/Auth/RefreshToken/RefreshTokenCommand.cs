using MediatR;
using Microsoft.AspNetCore.Identity;
using OnlineConsulting.Modules.Identity.Application.Features.Auth.Abstractions;
using OnlineConsulting.Modules.Identity.Application.Features.Auth.Contracts;
using OnlineConsulting.Modules.Identity.Domain;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Identity.Application.Features.Auth.RefreshToken;

public record RefreshTokenCommand(string AccessToken, string RefreshToken) : IRequest<OperationDataResult<AuthTokensResponse>>;

public class RefreshTokenHandler(UserManager<User> userManager, RoleManager<Role> roleManager, ITokenService tokenService, IRefreshTokenService refreshTokenService) : IRequestHandler<RefreshTokenCommand, OperationDataResult<AuthTokensResponse>>
{
    public async Task<OperationDataResult<AuthTokensResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var userId = tokenService.GetUserIdFromExpiredToken(request.AccessToken);

        if (userId is null || !Guid.TryParse(userId, out _))
            return Result.BadRequest<AuthTokensResponse>("The access token is invalid.");

        var user = await userManager.FindByIdAsync(userId);

        if (user is null || !await refreshTokenService.ValidateAsync(user, request.RefreshToken, cancellationToken))
            return Result.Unauthorized<AuthTokensResponse>("The refresh token is invalid or has expired.");

        var roles = await userManager.GetRolesAsync(user);
        var permissions = await RolePermissionResolver.ResolvePermissionsAsync(roleManager, roles);

        var (accessToken, accessTokenExpiresAt) = tokenService.CreateAccessToken(user, [.. roles], permissions);
        var (newRefreshToken, _) = await refreshTokenService.IssueAsync(user, cancellationToken);

        return Result.Success(new AuthTokensResponse(user.Id, accessToken, newRefreshToken, accessTokenExpiresAt), "Token refreshed successfully.");
    }
}
