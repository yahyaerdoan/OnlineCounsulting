namespace OnlineConsulting.UserInterface.Areas.User.Features.Equipment;

/// <summary>Raw Api calls for the customer's own equipment (/api/equipment/mine) and its service history
/// (/api/equipment/{id}/work-orders - unscoped server-side, so this service only ever calls it for ids that
/// GetMineAsync already returned for the current user, keeping it customer-safe).</summary>
public interface IUserEquipmentService
{
    Task<List<EquipmentResponse>> GetMineAsync(CancellationToken cancellationToken = default);
    Task<List<WorkOrderResponse>> GetWorkOrderHistoryAsync(Guid equipmentId, CancellationToken cancellationToken = default);
}

public record EquipmentResponse(Guid Id, Guid UserId, string Type, string? Brand, string? Model, string? SerialNumber, DateTimeOffset? InstallDate, DateTimeOffset? WarrantyExpiresAt, string? Notes);

public record WorkOrderResponse(Guid Id, Guid AppointmentId, Guid TechnicianUserId, string? PartsUsed, string? TechnicianNotes, DateTimeOffset? CompletedAt, Guid? EquipmentId);
