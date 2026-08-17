using Core.SecurityLayer.Hashings;
using Core.SecurityLayer.JsonWebTokens.Abstractions;
using OnlineConsulting.Modules.Identity.Application.Features.Auth.Contracts;
using OnlineConsulting.Modules.Identity.Application.Features.Auth.Abstractions;
using OnlineConsulting.Modules.Identity.Domain;
using System.Security.Cryptography;

namespace OnlineConsulting.Modules.Identity.Infrastructure.Security;

public class RefreshTokenService(IRefreshTokenRepository repository, IJwtTokenHelper jwtTokenHelper) : IRefreshTokenService
{
    public async Task<(string RawToken, DateTime ExpiresAt)> IssueAsync(User user, CancellationToken cancellationToken = default)
    {
        var refreshToken = jwtTokenHelper.CreateRefreshToken();

        var existing = await repository.GetAsync(rt => rt.UserId == user.Id, cancellationToken: cancellationToken);
        if (existing is not null)
        {
            existing.TokenHash = refreshToken.HashedToken;
            existing.ExpiresAt = refreshToken.Expires;
            await repository.UpdateAsync(existing);
        }
        else
        {
            await repository.AddAsync(new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                TokenHash = refreshToken.HashedToken,
                ExpiresAt = refreshToken.Expires,
            });
        }

        return (refreshToken.RawToken, refreshToken.Expires);
    }

    public async Task<bool> ValidateAsync(User user, string rawToken, CancellationToken cancellationToken = default)
    {
        var stored = await repository.GetAsync(rt => rt.UserId == user.Id, cancellationToken: cancellationToken);

        if (stored is null || stored.ExpiresAt < DateTime.UtcNow)
            return false;

        // Constant-time compare to avoid a timing side-channel - Core.SecurityLayer only provides the hash.
        return CryptographicOperations.FixedTimeEquals(Convert.FromHexString(TokenHashingHelper.Hash(rawToken)), Convert.FromHexString(stored.TokenHash));
    }
}
