using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.GalleryCategories.Contracts;
using OnlineConsulting.Modules.SiteContent.Domain;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.GalleryItems.Contracts;

public record GalleryItemResponse(
    Guid Id,
    string Description,
    Guid? PhotoMediaAssetId,
    int DisplayOrder,
    Dictionary<string, object>? Metadata,
    List<GalleryCategoryResponse> Categories)
{
    public static GalleryItemResponse FromDomain(GalleryItem entity, List<GalleryCategoryResponse> categories) => new(
        entity.Id, entity.Description, entity.PhotoMediaAssetId, entity.DisplayOrder,
        MetadataSerializer.Deserialize(entity.Metadata), categories);
}
