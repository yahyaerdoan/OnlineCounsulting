using OnlineConsulting.UserInterface.Infrastructure.Api;
using OnlineConsulting.UserInterface.Features.Gallery;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.GalleryCategory;

public class AdminGalleryCategoryService(IGalleryService galleryService) : IAdminGalleryCategoryService
{
    public async Task<List<GalleryCategoryListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var categories = await galleryService.GetCategoriesAsync(cancellationToken);
        return categories.Select(c => new GalleryCategoryListItemViewModel(c.Id, c.Name, c.Description)).ToList();
    }

    public async Task<UpdateGalleryCategoryViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // The Api exposes the tag list only - there is no single-tag endpoint, so the item is picked out of it.
        var categories = await galleryService.GetCategoriesAsync(cancellationToken);
        var category = categories.FirstOrDefault(c => c.Id == id);
        if (category is null)
            return null;

        return new UpdateGalleryCategoryViewModel { Id = category.Id, Name = category.Name, Description = category.Description };
    }

    public async Task<ApiEnvelope> CreateAsync(CreateGalleryCategoryViewModel model, CancellationToken cancellationToken = default) =>
        (await galleryService.CreateCategoryAsync(model.Name, model.Description, cancellationToken)).WithoutData();

    public Task<ApiEnvelope> UpdateAsync(UpdateGalleryCategoryViewModel model, CancellationToken cancellationToken = default) =>
        galleryService.UpdateCategoryAsync(model.Id, model.Name, model.Description, cancellationToken);

    public Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default) =>
        galleryService.DeleteCategoryAsync(id, cancellationToken);
}
