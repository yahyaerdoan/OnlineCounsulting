namespace OnlineConsulting.Modules.Tenancy.Infrastructure.Cleanup;

/// <summary>Config shape mirrors OutboxDispatcherOptions (see OnlineConsulting.Notifications.Dispatch.OutboxDispatcherOptions) - bound from the Tenancy:OrphanCleanup config section.</summary>
public class TenancyCleanupOptions
{
    public TimeSpan PollInterval { get; set; } = TimeSpan.FromHours(1);

    /// <summary>How long a Tenant may sit in PendingPayment/Failed with no admin User before OrphanedTenantCleanupService considers it abandoned and reaps it. Conservative default on purpose - this is a cleanup job, and wrongly deleting a live-but-slow signup is worse than leaving clutter behind.</summary>
    public TimeSpan GracePeriod { get; set; } = TimeSpan.FromHours(24);
}
