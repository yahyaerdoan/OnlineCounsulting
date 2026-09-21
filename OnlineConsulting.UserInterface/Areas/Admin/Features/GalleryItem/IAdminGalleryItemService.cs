using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.GalleryItem;

/// <summary>View-model orchestration for the gallery item admin screens - composes Services.IGalleryService
/// (Api CRUD) with IMediaService (photo upload/url resolution).</summary>
public interface IAdminGalleryItemService
{
    Task<List<GalleryItemListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<CreateGalleryItemViewModel> BuildCreateModelAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets one gallery item for editing, or null if not found. The Api has no single-item endpoint, so this is picked out of the full list.</summary>
    Task<UpdateGalleryItemViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Populates the available category options on the model, for re-rendering the form after validation failure.</summary>
    Task FillCategoriesAsync(CreateGalleryItemViewModel model, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> CreateAsync(CreateGalleryItemViewModel model, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> UpdateAsync(UpdateGalleryItemViewModel model, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
