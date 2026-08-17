using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Service;

/// <summary>Api orchestration for the Service admin screens. The old ServiceImageController is folded in here:
/// the Services module replaced the ServiceImage table with CoverMediaAssetId + ServiceMediaItems, so adding and
/// removing photos are now just two more actions on this one feature (AddImageAsync/RemoveImageAsync).</summary>
public interface IAdminServiceCatalogService
{
    Task<List<ServiceListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<CreateServiceViewModel> BuildCreateModelAsync(CancellationToken cancellationToken = default);
    Task<UpdateServiceViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task FillCategoriesAsync(CreateServiceViewModel model, CancellationToken cancellationToken = default);

    Task<ApiEnvelope> CreateAsync(CreateServiceViewModel model, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> UpdateAsync(UpdateServiceViewModel model, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Uploads the file to Media and attaches it to the service - becomes the cover if the service has none yet.</summary>
    Task<ApiEnvelope> AddImageAsync(Guid serviceId, IFormFile? image, CancellationToken cancellationToken = default);

    /// <summary>imageId is either a ServiceMediaItem id (removed) or the service's cover media asset id (cover cleared).</summary>
    Task<ApiEnvelope> RemoveImageAsync(Guid serviceId, Guid imageId, CancellationToken cancellationToken = default);
}
