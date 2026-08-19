namespace OnlineConsulting.Modules.Tenancy.Domain;

/// <summary>TenantSubscriptionItem.Status vocabulary.</summary>
public static class TenantSubscriptionItemStatuses
{
    public const string Pending = "Pending";
    public const string Active = "Active";

    /// <summary>See TenantStatuses.Failed - same terminal-but-recorded meaning, for one module line item.</summary>
    public const string Failed = "Failed";
}
