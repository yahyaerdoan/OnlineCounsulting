using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Memberships.Domain;

public class CustomerMembership : SequentialGuidTenantEntity
{
    /// <summary>Plain id, no navigation - User lives in the Identity module's own DbContext.</summary>
    public required Guid UserId { get; set; }

    /// <summary>Plain id, no navigation, same cross-module convention as Appointment.ServiceId.</summary>
    public required Guid MembershipPlanId { get; set; }

    public required string Status { get; set; }
    public required DateTimeOffset StartDate { get; set; }
    public DateTimeOffset? RenewalDate { get; set; }

    public string? ProviderCustomerId { get; set; }
    public string? ProviderSubscriptionId { get; set; }

    /// <summary>True once the member has requested cancellation - Status stays Active (they keep access
    /// through RenewalDate) until the provider's webhook fires at the real period end and
    /// OnSubscriptionCancelledHandler flips Status to Cancelled.</summary>
    public bool CancelAtPeriodEnd { get; set; }

    /// <summary>When this membership first became PastDue - null once back to Active. Drives
    /// MembershipGracePeriodCleanupService's grace-period cutoff.</summary>
    public DateTimeOffset? PastDueSince { get; set; }

    /// <summary>Set once at subscribe time from MembershipPlan.TrialDays - never touched again. Purely
    /// informational (Status stays Active during the trial, same "extra fact" pattern as CancelAtPeriodEnd).</summary>
    public DateTimeOffset? TrialEndDate { get; set; }

    /// <summary>Set once at subscribe time if a promo code was applied - never touched again. Used to
    /// block the same user from redeeming the same code again on a future resubscribe.</summary>
    public Guid? PromoCodeId { get; set; }
}
