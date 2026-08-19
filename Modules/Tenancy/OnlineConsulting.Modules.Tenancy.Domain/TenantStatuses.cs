namespace OnlineConsulting.Modules.Tenancy.Domain;

/// <summary>Tenant.Status vocabulary.</summary>
public static class TenantStatuses
{
    public const string PendingPayment = "PendingPayment";
    public const string Active = "Active";
    public const string PastDue = "PastDue";
    public const string Suspended = "Suspended";
    public const string Cancelled = "Cancelled";

    /// <summary>Signup failed after the provider-side customer/subscription calls started - kept as a terminal, recorded state (rather than deleting the row) so a failed Stripe charge always has a local trace and a retried signup can find and resume it.</summary>
    public const string Failed = "Failed";
}
