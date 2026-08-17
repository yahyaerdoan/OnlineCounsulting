using OnlineConsulting.UserInterface.Infrastructure.Api;
using OnlineConsulting.UserInterface.Features.Gallery;
using OnlineConsulting.UserInterface.Infrastructure.Media;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.GalleryItem;

public class AdminGalleryItemService(IGalleryService galleryService, IMediaService mediaService) : IAdminGalleryItemService
{
    public async Task<List<GalleryItemListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var items = await galleryService.GetItemsAsync(cancellationToken);

        var listItems = new List<GalleryItemListItemViewModel>();
        foreach (var item in items)
        {
            listItems.Add(new GalleryItemListItemViewModel(
                item.Id,
                item.Description,
                await mediaService.ResolveUrlAsync(item.PhotoMediaAssetId, cancellationToken),
                item.Categories.Select(c => c.Name).ToList()));
        }

        return listItems;
    }

    public async Task<CreateGalleryItemViewModel> BuildCreateModelAsync(CancellationToken cancellationToken = default)
    {
        var model = new CreateGalleryItemViewModel();
        await FillCategoriesAsync(model, cancellationToken);
        return model;
    }

    public async Task<UpdateGalleryItemViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // The Api exposes the item list only - there is no single-item endpoint, so the item is picked out of it.
        var items = await galleryService.GetItemsAsync(cancellationToken);
        var item = items.FirstOrDefault(i => i.Id == id);
        if (item is null)
            return null;

        var model = new UpdateGalleryItemViewModel
        {
            Id = item.Id,
            Description = item.Description,
            GalleryCategoryIds = item.Categories.Select(c => c.Id).ToList(),
            PhotoMediaAssetId = item.PhotoMediaAssetId,
            ImageUrl = await mediaService.ResolveUrlAsync(item.PhotoMediaAssetId, cancellationToken),
            DisplayOrder = item.DisplayOrder,
        };

        await FillCategoriesAsync(model, cancellationToken);
        return model;
    }

    public async Task FillCategoriesAsync(CreateGalleryItemViewModel model, CancellationToken cancellationToken = default)
    {
        var categories = await galleryService.GetCategoriesAsync(cancellationToken);
        model.AvailableCategories = categories.Select(c => new GalleryCategoryOptionViewModel(c.Id, c.Name)).ToList();
    }

    public async Task<ApiEnvelope> CreateAsync(CreateGalleryItemViewModel model, CancellationToken cancellationToken = default)
    {
        var photoMediaAssetId = await mediaService.UploadAsync(model.Image, cancellationToken);
        var result = await galleryService.CreateItemAsync(model.Description, model.GalleryCategoryIds, photoMediaAssetId, model.DisplayOrder, cancellationToken: cancellationToken);
        return result.WithoutData();
    }

    public async Task<ApiEnvelope> UpdateAsync(UpdateGalleryItemViewModel model, CancellationToken cancellationToken = default)
    {
        var uploadedId = await mediaService.UploadAsync(model.Image, cancellationToken);
        return await galleryService.UpdateItemAsync(model.Id, model.Description, model.GalleryCategoryIds,
            uploadedId ?? model.PhotoMediaAssetId, model.DisplayOrder, cancellationToken: cancellationToken);
    }

    public Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default) =>
        galleryService.DeleteItemAsync(id, cancellationToken);
}
