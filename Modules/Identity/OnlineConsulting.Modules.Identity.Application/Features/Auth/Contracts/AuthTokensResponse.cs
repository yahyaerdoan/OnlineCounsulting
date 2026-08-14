namespace OnlineConsulting.Modules.Identity.Application.Features.Auth.Contracts;

public record AuthTokensResponse(Guid UserId, string AccessToken, string RefreshToken, DateTime AccessTokenExpiresAt);
