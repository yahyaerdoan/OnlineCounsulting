using OnlineConsulting.UserInterface.Infrastructure.Media;

namespace OnlineConsulting.UserInterface.Features.Gallery;

public class GalleryContentService(IGalleryService galleryService, IMediaService mediaService) : IGalleryContentService
{
    public Task<List<GalleryCategoryResponse>> GetCategoriesAsync(CancellationToken cancellationToken = default) =>
        galleryService.GetCategoriesAsync(cancellationToken);

    public async Task<List<GalleryItemViewModel>> GetItemsAsync(CancellationToken cancellationToken = default)
    {
        var items = await galleryService.GetItemsAsync(cancellationToken);

        var result = new List<GalleryItemViewModel>();
        foreach (var item in items)
        {
            var photoUrl = await mediaService.ResolveUrlAsync(item.PhotoMediaAssetId, cancellationToken);
            result.Add(new GalleryItemViewModel(item.Id, item.Description, photoUrl, item.Categories));
        }

        return result;
    }
}
