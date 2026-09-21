using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Equipment;

/// <summary>Equipment orchestration for admin screens; equipment has no customer self-service write path - it's always admin/technician-recorded.</summary>
public interface IEquipmentService
{
    /// <summary>Lists all equipment with customer names resolved.</summary>
    Task<List<EquipmentListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets one equipment item for editing, or null if not found.</summary>
    Task<UpdateEquipmentViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<ApiEnvelope> CreateAsync(CreateEquipmentViewModel model, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> UpdateAsync(UpdateEquipmentViewModel model, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Lists customers for the equipment-owner dropdown.</summary>
    Task<List<CustomerOptionViewModel>> GetCustomerOptionsAsync(CancellationToken cancellationToken = default);
}
