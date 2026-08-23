namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors POST /api/auth/login's response shape.</summary>
public record AuthTokensResponse(Guid UserId, string AccessToken, string RefreshToken, DateTime AccessTokenExpiresAt);
