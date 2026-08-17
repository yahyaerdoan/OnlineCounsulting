
namespace OnlineConsulting.UserInterface.Features.Gallery;

/// <summary>Api orchestration for the public gallery widgets - composes IGalleryService (categories/items) with
/// IMediaService (photo url resolution) so the ViewComponents only ever call this one interface.</summary>
public interface IGalleryContentService
{
    Task<List<GalleryCategoryResponse>> GetCategoriesAsync(CancellationToken cancellationToken = default);
    Task<List<GalleryItemViewModel>> GetItemsAsync(CancellationToken cancellationToken = default);
}
