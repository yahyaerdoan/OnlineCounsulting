using OnlineConsulting.Modules.Identity.Domain;

namespace OnlineConsulting.Modules.Identity.Application.Features.Auth.Abstractions;

/// <summary>One refresh token per sign-in session, so signing in elsewhere never signs other devices out.</summary>
public interface IRefreshTokenService
{
    /// <summary>Starts a new session for the user; the user's other sessions stay valid.</summary>
    Task<(string RawToken, DateTime ExpiresAt)> IssueAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>Replaces the session's token with a new one; null when the token is unknown, expired or belongs to someone else.</summary>
    Task<(string RawToken, DateTime ExpiresAt)?> RotateAsync(User user, string rawToken, CancellationToken cancellationToken = default);
}
