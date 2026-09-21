using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.ServiceArea;

/// <summary>All Api orchestration for the ServiceArea admin screens - ServiceAreaController only calls this and
/// renders the result, it never talks to IApiClient directly.</summary>
public interface IServiceAreaService
{
    /// <summary>Fetches all service areas.</summary>
    Task<List<ServiceAreaListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Fetches a single service area for editing, or null if not found.</summary>
    Task<UpdateServiceAreaViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Creates a new service area.</summary>
    Task<ApiEnvelope> CreateAsync(CreateServiceAreaViewModel model, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing service area.</summary>
    Task<ApiEnvelope> UpdateAsync(UpdateServiceAreaViewModel model, CancellationToken cancellationToken = default);

    /// <summary>Deletes a service area by id.</summary>
    Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
