
namespace OnlineConsulting.Modules.Memberships.Domain;

/// <summary>Membership lifecycle rules shared by <see cref="CustomerMembership"/> and code that only has its state (e.g. HATEOAS links).</summary>
public static class CustomerMembershipRules
{
    /// <summary>Ended for good; a new subscription is needed.</summary>
    public static bool IsCancelled(string status) => status == CustomerMembershipStatuses.Cancelled;

    /// <summary>Signup not finished: the first payment is pending or the attempt failed.</summary>
    public static bool IsAwaitingFirstPayment(string status) => status is CustomerMembershipStatuses.PendingPayment or CustomerMembershipStatuses.Failed;

    /// <summary>Only an active membership can be paused.</summary>
    public static bool CanBePaused(string status) => status == CustomerMembershipStatuses.Active;

    /// <summary>Only a paused membership can be resumed.</summary>
    public static bool CanBeResumed(string status) => status == CustomerMembershipStatuses.Paused;

    /// <summary>Active and not set to end.</summary>
    public static bool CanChangePlan(string status, bool cancelAtPeriodEnd) => status == CustomerMembershipStatuses.Active && !cancelAtPeriodEnd;

    /// <summary>Not cancelled and not already set to end.</summary>
    public static bool CanBeCancelledAtPeriodEnd(string status, bool cancelAtPeriodEnd) => !IsCancelled(status) && !cancelAtPeriodEnd;

    /// <summary>Set to end but not cancelled yet, so the cancellation can be undone.</summary>
    public static bool CanBeReactivated(string status, bool cancelAtPeriodEnd) => !IsCancelled(status) && cancelAtPeriodEnd;
}
