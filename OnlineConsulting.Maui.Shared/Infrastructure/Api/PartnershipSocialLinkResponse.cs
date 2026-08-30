namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Nested inside PartnershipResponse - never independently queried, so it doesn't implement IQueryableFields.</summary>
public record PartnershipSocialLinkResponse(Guid Id, string Name, string Url, string Icon, string? IconColor);
