
namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors POST /api/v1/auth/login's response shape.</summary>
public record AuthTokensResponse(Guid UserId, string AccessToken, string RefreshToken, DateTime AccessTokenExpiresAt);
