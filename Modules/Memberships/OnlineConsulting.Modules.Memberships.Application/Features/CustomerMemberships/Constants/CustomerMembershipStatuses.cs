namespace OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Constants;

public static class CustomerMembershipStatuses
{
    /// <summary>Subscription created on the provider but not yet confirmed active (e.g. Stripe's "incomplete" - the initial invoice payment needs extra action).</summary>
    public const string PendingPayment = "PendingPayment";

    public const string Active = "Active";
    public const string PastDue = "PastDue";
    public const string Cancelled = "Cancelled";

    /// <summary>Signup failed after the provider-side customer/subscription calls started - kept as a terminal, recorded state (rather than deleting the row) so a failed Stripe charge always has a local trace and a retried subscribe can find and resume it.</summary>
    public const string Failed = "Failed";
}
