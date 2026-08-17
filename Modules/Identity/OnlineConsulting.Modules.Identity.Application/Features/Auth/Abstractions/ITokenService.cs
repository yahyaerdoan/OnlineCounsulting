using OnlineConsulting.Modules.Identity.Domain;

namespace OnlineConsulting.Modules.Identity.Application.Features.Auth.Abstractions;

public interface ITokenService
{
    (string Token, DateTime ExpiresAt) CreateAccessToken(User user, IReadOnlyList<string> roles, IReadOnlyList<string> permissions);

    /// <summary>Reads the user id from an access token without validating expiry - for the refresh flow. Null if the signature doesn't validate.</summary>
    string? GetUserIdFromExpiredToken(string accessToken);
}
