using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.GalleryCategory;

/// <summary>View-model orchestration for the gallery category (tag) admin screens on top of the shared
/// Services.IGalleryService Api wrapper.</summary>
public interface IAdminGalleryCategoryService
{
    Task<List<GalleryCategoryListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UpdateGalleryCategoryViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> CreateAsync(CreateGalleryCategoryViewModel model, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> UpdateAsync(UpdateGalleryCategoryViewModel model, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
