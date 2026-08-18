using Hateoas;
using OnlineConsulting.Modules.Scheduling.Domain;

namespace OnlineConsulting.Modules.Scheduling.Application.Features.WorkOrders.Contracts;

/// <summary>A class with required init properties instead of a positional record, since records can't inherit LinkedResponse.</summary>
public class WorkOrderResponse : LinkedResponse
{
    public required Guid Id { get; init; }
    public required Guid AppointmentId { get; init; }
    public required Guid TechnicianUserId { get; init; }
    public string? PartsUsed { get; init; }
    public string? TechnicianNotes { get; init; }
    public DateTimeOffset? CompletedAt { get; init; }
    public Guid? EquipmentId { get; init; }
    public required List<WorkOrderMediaItemResponse> MediaItems { get; init; }

    public static WorkOrderResponse FromDomain(WorkOrder workOrder, IEnumerable<WorkOrderMediaItem> mediaItems) => new()
    {
        Id = workOrder.Id,
        AppointmentId = workOrder.AppointmentId,
        TechnicianUserId = workOrder.TechnicianUserId,
        PartsUsed = workOrder.PartsUsed,
        TechnicianNotes = workOrder.TechnicianNotes,
        CompletedAt = workOrder.CompletedAt,
        EquipmentId = workOrder.EquipmentId,
        MediaItems = [.. mediaItems.OrderBy(m => m.DisplayOrder).Select(WorkOrderMediaItemResponse.FromDomain)],
    };
}
