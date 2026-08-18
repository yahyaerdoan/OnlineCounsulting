namespace OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Constants;

public static class CustomerMembershipStatuses
{
    /// <summary>Subscription created on the provider but not yet confirmed active (e.g. Stripe's "incomplete" - the initial invoice payment needs extra action).</summary>
    public const string PendingPayment = "PendingPayment";

    public const string Active = "Active";
    public const string PastDue = "PastDue";
    public const string Cancelled = "Cancelled";
}
