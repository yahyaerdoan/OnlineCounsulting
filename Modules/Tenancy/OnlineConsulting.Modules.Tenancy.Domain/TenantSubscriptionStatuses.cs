namespace OnlineConsulting.Modules.Tenancy.Domain;

/// <summary>Values of <see cref="TenantSubscription.Status"/>.</summary>
public static class TenantSubscriptionStatuses
{
    public const string PendingPayment = "PendingPayment";
    public const string Active = "Active";
    public const string PastDue = "PastDue";
    public const string Cancelled = "Cancelled";

    /// <summary>Signup failed; kept so a retried signup can resume it.</summary>
    public const string Failed = "Failed";
}
