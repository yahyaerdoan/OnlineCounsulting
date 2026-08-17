namespace OnlineConsulting.UserInterface.Infrastructure.Api;

/// <summary>Local wire-contract copy of the Api's AuthTokensResponse (Modules.Identity.Application) - UserInterface calls the Api's /api/auth/login over HTTP rather than referencing that Application project's types directly, same reasoning as the SiteContent/Media DTOs in PartnershipController.</summary>
public record AuthTokensResponseDto(Guid UserId, string AccessToken, string RefreshToken, DateTime AccessTokenExpiresAt);
