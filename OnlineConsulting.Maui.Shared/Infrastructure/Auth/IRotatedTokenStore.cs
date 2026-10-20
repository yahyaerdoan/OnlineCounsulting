namespace OnlineConsulting.Maui.Shared.Infrastructure.Auth;

/// <summary>Old refresh token to its replacement, so the cookie and other circuits catch up; web only.</summary>
public interface IRotatedTokenStore
{
    void Record(string oldRefreshToken, TokenSet replacement);

    /// <summary>Newest replacement, following chained rotations; null if never rotated.</summary>
    TokenSet? FindLatest(string refreshToken);
}
