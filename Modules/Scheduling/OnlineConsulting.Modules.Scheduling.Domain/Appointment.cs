using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Scheduling.Domain;

public class Appointment : TenantEntity<Guid>
{
    /// <summary>Plain id, no navigation - User lives in the Identity module's own DbContext.</summary>
    public required Guid UserId { get; set; }

    /// <summary>Plain id, no navigation, same cross-module convention as BasketItem.ServiceId. Null means a generic meeting request with the tenant, not tied to a bookable service.</summary>
    public Guid? ServiceId { get; set; }

    public required DateTimeOffset ScheduledStart { get; set; }
    public required DateTimeOffset ScheduledEnd { get; set; }
    public required string Status { get; set; }
    public string? CustomerNote { get; set; }

    /// <summary>Freeform, customer-entered - where the technician needs to physically go. Kept as plain text (not a structured address) since its only real consumer is a maps deep link, which accepts free text just as well as a structured address.</summary>
    public string? ServiceAddress { get; set; }

    /// <summary>Snapshot of Service.RequiresPrepayment at booking time. Not enforced yet - no payment gateway exists - but the field exists now so a future PendingPayment status can gate on it without another migration.</summary>
    public bool RequiresPrepayment { get; set; }

    /// <summary>Plain id, no navigation - User lives in the Identity module's own DbContext. Set by AssignTechnicianCommand ahead of the visit (dispatch), not at WorkOrder time - this is what authorizes a technician to join the live-tracking hub group and push GPS updates for this appointment.</summary>
    public Guid? AssignedTechnicianUserId { get; set; }
}
