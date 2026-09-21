using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.ModuleOffering;

public interface IModuleOfferingService
{
    /// <summary>Fetches all module offerings.</summary>
    Task<List<ModuleOfferingListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Fetches a single offering for editing, or null if not found.</summary>
    Task<UpdateModuleOfferingViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Creates a new module offering.</summary>
    Task<ApiEnvelope> CreateAsync(CreateModuleOfferingViewModel model, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing offering's editable fields (name, visibility).</summary>
    Task<ApiEnvelope> UpdateAsync(UpdateModuleOfferingViewModel model, CancellationToken cancellationToken = default);
}
