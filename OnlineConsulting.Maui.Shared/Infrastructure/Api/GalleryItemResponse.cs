namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors POST /api/site-content/gallery-items/query's response shape.</summary>
public record GalleryItemResponse(Guid Id, string Description, Guid? PhotoMediaAssetId, int DisplayOrder, List<GalleryCategoryResponse> Categories) : IQueryableFields
{
    public static string[] SearchFields => [nameof(Description)];
}
