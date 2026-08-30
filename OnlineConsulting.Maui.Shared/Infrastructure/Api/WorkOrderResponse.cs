namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors Scheduling's WorkOrderResponse.</summary>
public record WorkOrderResponse(Guid Id, Guid AppointmentId, Guid TechnicianUserId, string? PartsUsed, string? TechnicianNotes,
    DateTimeOffset? CompletedAt, Guid? EquipmentId, List<WorkOrderMediaItemResponse> MediaItems);

/// <summary>Mirrors Scheduling's WorkOrderMediaItemResponse.</summary>
public record WorkOrderMediaItemResponse(Guid Id, Guid MediaAssetId, bool IsBeforePhoto, int DisplayOrder);
