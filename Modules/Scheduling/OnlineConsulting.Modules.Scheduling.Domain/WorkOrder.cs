using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Scheduling.Domain;

/// <summary>1-1 with Appointment - plain id, no navigation, same cross-module/intra-module convention as Appointment.ServiceId. Recording a WorkOrder is what actually completes the Appointment (see CreateWorkOrderHandler).</summary>
public class WorkOrder : TenantEntity<Guid>
{
    public required Guid AppointmentId { get; set; }

    /// <summary>Plain id, no navigation - User lives in the Identity module's own DbContext.</summary>
    public required Guid TechnicianUserId { get; set; }

    public string? PartsUsed { get; set; }
    public string? TechnicianNotes { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }

    /// <summary>Plain id, no navigation - EquipmentItem lives in the Equipment module's own DbContext. Null when the job wasn't tied to a specific piece of the customer's equipment.</summary>
    public Guid? EquipmentId { get; set; }
}
