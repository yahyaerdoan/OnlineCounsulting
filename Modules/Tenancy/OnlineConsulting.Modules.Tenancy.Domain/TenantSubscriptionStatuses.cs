namespace OnlineConsulting.Modules.Tenancy.Domain;

/// <summary>TenantSubscription.Status vocabulary.</summary>
public static class TenantSubscriptionStatuses
{
    public const string PendingPayment = "PendingPayment";
    public const string Active = "Active";
    public const string PastDue = "PastDue";
    public const string Cancelled = "Cancelled";

    /// <summary>See TenantStatuses.Failed - same terminal-but-recorded meaning, for the subscription row.</summary>
    public const string Failed = "Failed";
}
