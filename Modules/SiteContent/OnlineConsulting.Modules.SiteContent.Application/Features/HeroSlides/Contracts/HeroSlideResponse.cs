using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Domain;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.HeroSlides.Contracts;

public record HeroSlideResponse(Guid Id, string Title, string Description, string ImageUrl, int DisplayOrder, Dictionary<string, object>? Metadata)
{
    public static HeroSlideResponse FromDomain(HeroSlide entity) => new(entity.Id, entity.Title, entity.Description, entity.ImageUrl, entity.DisplayOrder, MetadataSerializer.Deserialize(entity.Metadata));
}
