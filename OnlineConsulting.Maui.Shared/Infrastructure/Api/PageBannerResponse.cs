namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors GET /api/site-content/page-banners's response shape. Metadata deliberately
/// omitted - nothing in the admin UI reads or writes it.</summary>
public record PageBannerResponse(Guid Id, string Title, string Description, string ImageUrl, int DisplayOrder);
