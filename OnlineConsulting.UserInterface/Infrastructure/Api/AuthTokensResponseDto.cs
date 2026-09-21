namespace OnlineConsulting.UserInterface.Infrastructure.Api;

/// <summary>Local wire-contract copy of the Api's AuthTokensResponse - avoids referencing the Application project's types directly.</summary>
public record AuthTokensResponseDto(Guid UserId, string AccessToken, string RefreshToken, DateTime AccessTokenExpiresAt);
