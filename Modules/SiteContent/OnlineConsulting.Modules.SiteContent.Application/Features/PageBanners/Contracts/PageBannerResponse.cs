using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Domain;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.PageBanners.Contracts;

public record PageBannerResponse(Guid Id, string Title, string Description, string ImageUrl, int DisplayOrder, Dictionary<string, object>? Metadata)
{
    public static PageBannerResponse FromDomain(PageBanner entity) => new(entity.Id, entity.Title, entity.Description, entity.ImageUrl, entity.DisplayOrder, MetadataSerializer.Deserialize(entity.Metadata));
}
