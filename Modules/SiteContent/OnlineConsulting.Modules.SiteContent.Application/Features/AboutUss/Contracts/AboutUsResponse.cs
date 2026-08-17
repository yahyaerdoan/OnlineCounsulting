using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Domain;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.AboutUss.Contracts;

public record AboutUsResponse(Guid Id, string Title, string Description, string? CoverImage, string? VideoUrl, int DisplayOrder, Dictionary<string, object>? Metadata)
{
    public static AboutUsResponse FromDomain(AboutUs entity) => new(entity.Id, entity.Title, entity.Description, entity.CoverImage, entity.VideoUrl, entity.DisplayOrder, MetadataSerializer.Deserialize(entity.Metadata));
}
