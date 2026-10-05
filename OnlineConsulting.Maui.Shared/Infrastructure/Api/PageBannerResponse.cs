using OnlineConsulting.Maui.Shared.Infrastructure.Hateoas;

namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors GET /api/v1/site-content/page-banners - Metadata omitted, unused by the UI.</summary>
public record PageBannerResponse(Guid Id, string Title, string Description, string ImageUrl, int DisplayOrder) : HalResource;
