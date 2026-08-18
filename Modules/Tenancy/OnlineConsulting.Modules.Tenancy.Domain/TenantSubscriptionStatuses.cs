namespace OnlineConsulting.Modules.Tenancy.Domain;

/// <summary>TenantSubscription.Status vocabulary.</summary>
public static class TenantSubscriptionStatuses
{
    public const string PendingPayment = "PendingPayment";
    public const string Active = "Active";
    public const string PastDue = "PastDue";
    public const string Cancelled = "Cancelled";
}
