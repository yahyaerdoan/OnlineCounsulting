namespace OnlineConsulting.Modules.Tenancy.Domain;

/// <summary>Values of <see cref="Tenant.Status"/>.</summary>
public static class TenantStatuses
{
    public const string PendingPayment = "PendingPayment";
    public const string Active = "Active";
    public const string PastDue = "PastDue";
    public const string Suspended = "Suspended";
    public const string Cancelled = "Cancelled";

    /// <summary>Signup failed; kept so a retried signup can resume it.</summary>
    public const string Failed = "Failed";
}
