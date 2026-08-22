using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.ModuleOffering;

public interface IModuleOfferingService
{
    Task<List<ModuleOfferingListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UpdateModuleOfferingViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> CreateAsync(CreateModuleOfferingViewModel model, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> UpdateAsync(UpdateModuleOfferingViewModel model, CancellationToken cancellationToken = default);
}
