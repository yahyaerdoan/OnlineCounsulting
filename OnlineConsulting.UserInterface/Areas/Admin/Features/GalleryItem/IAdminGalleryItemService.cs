using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.GalleryItem;

/// <summary>View-model orchestration for the gallery item admin screens - composes Services.IGalleryService
/// (Api CRUD) with IMediaService (photo upload/url resolution).</summary>
public interface IAdminGalleryItemService
{
    Task<List<GalleryItemListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<CreateGalleryItemViewModel> BuildCreateModelAsync(CancellationToken cancellationToken = default);
    Task<UpdateGalleryItemViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task FillCategoriesAsync(CreateGalleryItemViewModel model, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> CreateAsync(CreateGalleryItemViewModel model, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> UpdateAsync(UpdateGalleryItemViewModel model, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
