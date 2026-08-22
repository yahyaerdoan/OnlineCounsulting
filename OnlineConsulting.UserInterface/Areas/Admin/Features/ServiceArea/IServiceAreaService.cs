using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.ServiceArea;

/// <summary>All Api orchestration for the ServiceArea admin screens - ServiceAreaController only calls this and
/// renders the result, it never talks to IApiClient directly.</summary>
public interface IServiceAreaService
{
    Task<List<ServiceAreaListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UpdateServiceAreaViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> CreateAsync(CreateServiceAreaViewModel model, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> UpdateAsync(UpdateServiceAreaViewModel model, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
