using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Equipment;

/// <summary>All Api orchestration for the Equipment admin screens - EquipmentController only calls this and
/// renders the result, it never talks to IApiClient directly. Equipment has no customer self-service write
/// path - it's always admin/technician-recorded (see UpdateEquipmentItemCommand: no UserId, ownership is
/// fixed at creation).</summary>
public interface IEquipmentService
{
    Task<List<EquipmentListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UpdateEquipmentViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> CreateAsync(CreateEquipmentViewModel model, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> UpdateAsync(UpdateEquipmentViewModel model, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<CustomerOptionViewModel>> GetCustomerOptionsAsync(CancellationToken cancellationToken = default);
}
