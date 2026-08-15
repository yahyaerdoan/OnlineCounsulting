using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Domain;

namespace OnlineConsulting.Modules.SiteContent.Application.Contracts;

public record FeatureHighlightResponse(Guid Id, string Title, string Description, string ImageUrl, int DisplayOrder, Dictionary<string, object>? Metadata)
{
    public static FeatureHighlightResponse FromDomain(FeatureHighlight entity) => new(entity.Id, entity.Title, entity.Description, entity.ImageUrl, entity.DisplayOrder, MetadataSerializer.Deserialize(entity.Metadata));
}
