namespace OnlineConsulting.Modules.Tenancy.Domain;

/// <summary>Tenant.Status vocabulary.</summary>
public static class TenantStatuses
{
    public const string PendingPayment = "PendingPayment";
    public const string Active = "Active";
    public const string PastDue = "PastDue";
    public const string Suspended = "Suspended";
    public const string Cancelled = "Cancelled";

    /// <summary>Signup failed after provider-side calls started - kept as a terminal state, not deleted, so a retried signup can find and resume it.</summary>
    public const string Failed = "Failed";
}
