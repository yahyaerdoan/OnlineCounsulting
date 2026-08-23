namespace OnlineConsulting.Maui.Shared.Infrastructure.Auth;

public record TokenSet(string AccessToken, string RefreshToken, DateTime AccessTokenExpiresAt)
{
    public bool IsNearExpiry(TimeSpan buffer) => DateTime.UtcNow >= AccessTokenExpiresAt - buffer;
}
