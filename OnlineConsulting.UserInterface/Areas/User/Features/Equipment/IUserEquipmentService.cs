namespace OnlineConsulting.UserInterface.Areas.User.Features.Equipment;

/// <summary>Customer's own equipment and work-order history; work-order lookup is unscoped server-side, so only call it with ids GetMineAsync already returned.</summary>
public interface IUserEquipmentService
{
    Task<List<EquipmentResponse>> GetMineAsync(CancellationToken cancellationToken = default);
    Task<List<WorkOrderResponse>> GetWorkOrderHistoryAsync(Guid equipmentId, CancellationToken cancellationToken = default);
}

public record EquipmentResponse(Guid Id, Guid UserId, string Type, string? Brand, string? Model, string? SerialNumber, DateTimeOffset? InstallDate, DateTimeOffset? WarrantyExpiresAt, string? Notes);

public record WorkOrderResponse(Guid Id, Guid AppointmentId, Guid TechnicianUserId, string? PartsUsed, string? TechnicianNotes, DateTimeOffset? CompletedAt, Guid? EquipmentId);
