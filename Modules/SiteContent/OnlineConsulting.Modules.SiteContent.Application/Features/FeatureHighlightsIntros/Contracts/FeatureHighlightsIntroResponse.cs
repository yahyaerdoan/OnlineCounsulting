using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Domain;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.FeatureHighlightsIntros.Contracts;

public record FeatureHighlightsIntroResponse(Guid Id, string Description, Guid? CoverMediaAssetId, int DisplayOrder, Dictionary<string, object>? Metadata)
{
    public static FeatureHighlightsIntroResponse FromDomain(FeatureHighlightsIntro entity) => new(entity.Id, entity.Description, entity.CoverMediaAssetId, entity.DisplayOrder, MetadataSerializer.Deserialize(entity.Metadata));
}
