using OnlineConsulting.Modules.Identity.Domain;

namespace OnlineConsulting.Modules.Identity.Application.Features.Auth.Contracts;

public interface IRefreshTokenService
{
    /// <summary>Issues a new refresh token for the user, overwriting (and so invalidating) any previous one.</summary>
    Task<(string RawToken, DateTime ExpiresAt)> IssueAsync(User user, CancellationToken cancellationToken = default);

    Task<bool> ValidateAsync(User user, string rawToken, CancellationToken cancellationToken = default);
}
