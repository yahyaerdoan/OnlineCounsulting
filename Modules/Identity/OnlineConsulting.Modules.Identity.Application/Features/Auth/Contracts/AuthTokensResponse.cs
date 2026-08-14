namespace OnlineConsulting.Modules.Identity.Application.Features.Auth.Contracts;

public record AuthTokensResponse(string AccessToken, string RefreshToken, DateTime AccessTokenExpiresAt);
