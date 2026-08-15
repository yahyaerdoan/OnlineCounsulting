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

    /// <summary>Snapshot of Service.RequiresPrepayment at booking time. Not enforced yet - no payment gateway exists - but the field exists now so a future PendingPayment status can gate on it without another migration.</summary>
    public bool RequiresPrepayment { get; set; }
}
