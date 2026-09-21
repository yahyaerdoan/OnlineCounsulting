namespace OnlineConsulting.Maui.Shared.Infrastructure.Auth;

public record TokenSet(string AccessToken, string RefreshToken, DateTime AccessTokenExpiresAt)
{
    /// <summary>True once the token expires within the given buffer window.</summary>
    public bool IsNearExpiry(TimeSpan buffer) => DateTime.UtcNow >= AccessTokenExpiresAt - buffer;
}
