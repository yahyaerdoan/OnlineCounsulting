using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.GalleryCategory;

/// <summary>View-model orchestration for the gallery category (tag) admin screens on top of the shared
/// Services.IGalleryService Api wrapper.</summary>
public interface IAdminGalleryCategoryService
{
    Task<List<GalleryCategoryListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets one gallery category for editing, or null if not found. The Api has no single-item endpoint, so this is picked out of the full list.</summary>
    Task<UpdateGalleryCategoryViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<ApiEnvelope> CreateAsync(CreateGalleryCategoryViewModel model, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> UpdateAsync(UpdateGalleryCategoryViewModel model, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
