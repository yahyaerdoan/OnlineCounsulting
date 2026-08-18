namespace OnlineConsulting.Modules.Tenancy.Domain;

/// <summary>Tenant.Status vocabulary.</summary>
public static class TenantStatuses
{
    public const string PendingPayment = "PendingPayment";
    public const string Active = "Active";
    public const string PastDue = "PastDue";
    public const string Suspended = "Suspended";
    public const string Cancelled = "Cancelled";
}
