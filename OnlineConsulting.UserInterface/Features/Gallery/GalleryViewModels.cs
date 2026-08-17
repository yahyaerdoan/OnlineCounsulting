
namespace OnlineConsulting.UserInterface.Features.Gallery;

/// <summary>PhotoUrl is resolved server-side (GalleryItemResponse only carries the MediaAssetId).</summary>
public record GalleryItemViewModel(Guid Id, string Description, string? PhotoUrl, List<GalleryCategoryResponse> Categories);
