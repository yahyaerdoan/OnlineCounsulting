using Core.SecurityLayer.Hashings;
using Core.SecurityLayer.JsonWebTokens.Abstractions;
using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.Identity.Application.Features.Auth.Abstractions;
using OnlineConsulting.Modules.Identity.Domain;

namespace OnlineConsulting.Modules.Identity.Infrastructure.Security;

/// <summary>A row per session, found by the token's hash; rotation rewrites only that row.</summary>
public class RefreshTokenService(IRefreshTokenRepository repository, IJwtTokenHelper jwtTokenHelper) : IRefreshTokenService
{
    public async Task<(string RawToken, DateTime ExpiresAt)> IssueAsync(User user, CancellationToken cancellationToken = default)
    {
        // Drop this user's dead sessions so rows don't pile up.
        _ = await repository.Query().Where(rt => rt.UserId == user.Id && rt.ExpiresAt < DateTime.UtcNow).ExecuteDeleteAsync(cancellationToken);

        var refreshToken = jwtTokenHelper.CreateRefreshToken();
        _ = await repository.AddAsync(new RefreshToken
        {
            UserId = user.Id,
            TokenHash = refreshToken.HashedToken,
            ExpiresAt = refreshToken.Expires,
        }, cancellationToken: cancellationToken);

        return (refreshToken.RawToken, refreshToken.Expires);
    }

    public async Task<(string RawToken, DateTime ExpiresAt)?> RotateAsync(User user, string rawToken, CancellationToken cancellationToken = default)
    {
        var hash = TokenHashingHelper.Hash(rawToken);
        var session = await repository.GetAsync(rt => rt.UserId == user.Id && rt.TokenHash == hash && rt.ExpiresAt >= DateTime.UtcNow, cancellationToken: cancellationToken);
        if (session is null)
        {
            return null;
        }

        var refreshToken = jwtTokenHelper.CreateRefreshToken();
        session.TokenHash = refreshToken.HashedToken;
        session.ExpiresAt = refreshToken.Expires;
        _ = await repository.UpdateAsync(session, cancellationToken: cancellationToken);

        return (refreshToken.RawToken, refreshToken.Expires);
    }
}
