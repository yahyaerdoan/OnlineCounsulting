namespace OnlineConsulting.Modules.Memberships.Infrastructure.Cleanup;

/// <summary>Config shape mirrors Commerce's PendingOrderCleanupOptions - bound from the
/// Memberships:GracePeriod config section.</summary>
public class MembershipGracePeriodOptions
{
    public TimeSpan PollInterval { get; set; } = TimeSpan.FromHours(1);

    /// <summary>How long a membership may stay PastDue (from CustomerMembership.PastDueSince) before
    /// MembershipGracePeriodCleanupService gives up and cancels it.</summary>
    public TimeSpan GraceAfter { get; set; } = TimeSpan.FromDays(7);
}
