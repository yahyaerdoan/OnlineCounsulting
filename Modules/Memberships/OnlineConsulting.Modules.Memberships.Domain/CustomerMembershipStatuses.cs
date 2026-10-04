namespace OnlineConsulting.Modules.Memberships.Domain;

/// <summary>Values of <see cref="CustomerMembership.Status"/>.</summary>
public static class CustomerMembershipStatuses
{
    /// <summary>Subscription created but its first payment is not confirmed yet.</summary>
    public const string PendingPayment = "PendingPayment";

    public const string Active = "Active";
    public const string PastDue = "PastDue";
    public const string Cancelled = "Cancelled";
    public const string Paused = "Paused";

    /// <summary>Signup failed; kept so the attempt can be retried.</summary>
    public const string Failed = "Failed";
}
