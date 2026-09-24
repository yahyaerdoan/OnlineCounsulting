using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Scheduling.Domain;

/// <summary>A customer's booked or requested service visit - starts life as Pending regardless of type, then moves through the Status vocabulary in AppointmentStatuses.</summary>
public class Appointment : SequentialGuidTenantEntity
{
    /// <summary>Plain id, no navigation - User lives in the Identity module's own DbContext.</summary>
    public required Guid UserId { get; set; }

    /// <summary>Plain id, no navigation, same cross-module convention as BasketItem.ServiceId. Null means a generic meeting request with the tenant, not tied to a bookable service.</summary>
    public Guid? ServiceId { get; set; }

    public required DateTimeOffset ScheduledStart { get; set; }
    public required DateTimeOffset ScheduledEnd { get; set; }
    public required string Status { get; set; }
    public string? CustomerNote { get; set; }

    /// <summary>Freeform customer-entered address - kept as plain text since its only consumer is a maps deep link.</summary>
    public string? ServiceAddress { get; set; }

    /// <summary>Snapshot of Service.RequiresPrepayment at booking time - not enforced yet, added so a future PendingPayment status can use it without a migration.</summary>
    public bool RequiresPrepayment { get; set; }

    /// <summary>Plain id, no navigation - User lives in the Identity module's own DbContext. Set by AssignTechnicianCommand ahead of the visit (dispatch), not at WorkOrder time - this is what authorizes a technician to join the live-tracking hub group and push GPS updates for this appointment.</summary>
    public Guid? AssignedTechnicianUserId { get; set; }
}
