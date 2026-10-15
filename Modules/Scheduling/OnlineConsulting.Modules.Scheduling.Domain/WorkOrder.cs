using Core.PersistenceLayer.MultiTenancy;

namespace OnlineConsulting.Modules.Scheduling.Domain;

/// <summary>What a technician did on a visit. Recorded once per appointment through <see cref="RecordFor"/>, which also completes the visit.</summary>
public class WorkOrder : SequentialGuidTenantEntity
{
    private WorkOrder()
    {
    }

    public Guid AppointmentId { get; private set; }

    /// <summary>Identity module user id, no navigation.</summary>
    public Guid TechnicianUserId { get; private set; }

    public string? PartsUsed { get; private set; }
    public string? TechnicianNotes { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }

    /// <summary>Equipment module id, no navigation.</summary>
    public Guid? EquipmentId { get; private set; }

    /// <summary>Records the visit's work and completes the appointment. Requires a technician and an appointment that isn't closed.</summary>
    public static WorkOrder RecordFor(Appointment appointment, Guid technicianUserId, string? partsUsed, string? technicianNotes, DateTimeOffset completedAt,
        Guid? equipmentId)
    {
        if (technicianUserId == Guid.Empty)
        {
            throw new ArgumentException("A work order needs the technician who did the work.", nameof(technicianUserId));
        }

        appointment.Complete();

        return new WorkOrder
        {
            AppointmentId = appointment.Id,
            TechnicianUserId = technicianUserId,
            PartsUsed = string.IsNullOrWhiteSpace(partsUsed) ? null : partsUsed.Trim(),
            TechnicianNotes = string.IsNullOrWhiteSpace(technicianNotes) ? null : technicianNotes.Trim(),
            CompletedAt = completedAt,
            EquipmentId = equipmentId,
        };
    }
}
