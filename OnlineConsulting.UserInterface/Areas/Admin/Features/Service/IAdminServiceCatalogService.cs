using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Service;

/// <summary>Service admin orchestration; the old ServiceImageController is folded in as AddImageAsync/RemoveImageAsync.</summary>
public interface IAdminServiceCatalogService
{
    /// <summary>Fetches all services with resolved category titles and cover urls.</summary>
    Task<List<ServiceListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Builds an empty create form pre-populated with the category options.</summary>
    Task<CreateServiceViewModel> BuildCreateModelAsync(CancellationToken cancellationToken = default);

    /// <summary>Fetches a single service for editing, or null if not found.</summary>
    Task<UpdateServiceViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Populates the model's category dropdown options.</summary>
    Task FillCategoriesAsync(CreateServiceViewModel model, CancellationToken cancellationToken = default);

    /// <summary>Creates a service, uploading images first so the first upload becomes the cover in one call.</summary>
    Task<ApiEnvelope> CreateAsync(CreateServiceViewModel model, CancellationToken cancellationToken = default);

    /// <summary>Updates a service and attaches any newly uploaded images.</summary>
    Task<ApiEnvelope> UpdateAsync(UpdateServiceViewModel model, CancellationToken cancellationToken = default);

    /// <summary>Deletes a service by id.</summary>
    Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Uploads the file to Media and attaches it to the service - becomes the cover if the service has none yet.</summary>
    Task<ApiEnvelope> AddImageAsync(Guid serviceId, IFormFile? image, CancellationToken cancellationToken = default);

    /// <summary>imageId is either a ServiceMediaItem id (removed) or the service's cover media asset id (cover cleared).</summary>
    Task<ApiEnvelope> RemoveImageAsync(Guid serviceId, Guid imageId, CancellationToken cancellationToken = default);
}
