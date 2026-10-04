namespace OnlineConsulting.Modules.Tenancy.Domain;

/// <summary>Tenant lifecycle rules shared by <see cref="Tenant"/> and code that only has the status (e.g. HATEOAS links).</summary>
public static class TenantRules
{
    /// <summary>Signup not finished: payment pending or the attempt failed.</summary>
    public static bool IsAwaitingSignup(string status) => status is TenantStatuses.PendingPayment or TenantStatuses.Failed;

    /// <summary>Suspended or cancelled by staff; billing events don't change it.</summary>
    public static bool IsHeldByStaff(string status) => status is TenantStatuses.Suspended or TenantStatuses.Cancelled;

    /// <summary>Only an active or past-due tenant can be suspended.</summary>
    public static bool CanBeSuspended(string status) => status is TenantStatuses.Active or TenantStatuses.PastDue;

    /// <summary>Only a suspended tenant can be reactivated.</summary>
    public static bool CanBeReactivated(string status) => status == TenantStatuses.Suspended;

    /// <summary>Anything not cancelled yet can be cancelled.</summary>
    public static bool CanBeCancelled(string status) => status != TenantStatuses.Cancelled;
}
