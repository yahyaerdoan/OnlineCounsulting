namespace OnlineConsulting.Modules.Tenancy.Domain;

/// <summary>Values of <see cref="TenantSubscriptionItem.Status"/>.</summary>
public static class TenantSubscriptionItemStatuses
{
    public const string Pending = "Pending";
    public const string Active = "Active";

    /// <summary>Billing failed; kept so a retry can resume it under the same id.</summary>
    public const string Failed = "Failed";
}
