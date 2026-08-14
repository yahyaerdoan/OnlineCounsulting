using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.Identity.Application.Features.Auth.Constants;
using OnlineConsulting.Modules.Identity.Application.Features.Auth.Contracts;
using OnlineConsulting.Modules.Identity.Domain;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Identity.Application.Features.Auth.Login;

public record LoginCommand(string UserNameOrEmail, string Password) : IRequest<OperationDataResult<AuthTokensResponse>>;

public class LoginHandler(UserManager<User> userManager, SignInManager<User> signInManager, ITokenService tokenService, IRefreshTokenService refreshTokenService)
    : IRequestHandler<LoginCommand, OperationDataResult<AuthTokensResponse>>
{
    public async Task<OperationDataResult<AuthTokensResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByNameAsync(request.UserNameOrEmail)
            ?? await userManager.Users.FirstOrDefaultAsync(u => u.Email == request.UserNameOrEmail, cancellationToken);

        if (user is null)
            return Result.BadRequest<AuthTokensResponse>(AuthMessages.InvalidCredentials);

        var signInResult = await signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);

        if (signInResult.IsLockedOut)
            return Result.Forbidden<AuthTokensResponse>(AuthMessages.AccountLocked);

        if (!signInResult.Succeeded)
            return Result.BadRequest<AuthTokensResponse>(AuthMessages.InvalidCredentials);

        var roles = await userManager.GetRolesAsync(user);

        var (accessToken, accessTokenExpiresAt) = tokenService.CreateAccessToken(user, [.. roles]);

        var (refreshToken, _) = await refreshTokenService.IssueAsync(user, cancellationToken);

        return Result.Success(new AuthTokensResponse(user.Id, accessToken, refreshToken, accessTokenExpiresAt), "Credentials validated successfully.");
    }
}
