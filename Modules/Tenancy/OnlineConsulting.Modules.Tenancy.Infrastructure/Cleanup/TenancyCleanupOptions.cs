namespace OnlineConsulting.Modules.Tenancy.Infrastructure.Cleanup;

/// <summary>Config shape mirrors OutboxDispatcherOptions (see OnlineConsulting.Notifications.Dispatch.OutboxDispatcherOptions) - bound from the Tenancy:OrphanCleanup config section.</summary>
public class TenancyCleanupOptions
{
    public TimeSpan PollInterval { get; set; } = TimeSpan.FromHours(1);

    /// <summary>How long a Tenant may sit unclaimed in PendingPayment/Failed before OrphanedTenantCleanupService reaps it - conservative, since deleting a live signup is worse than clutter.</summary>
    public TimeSpan GracePeriod { get; set; } = TimeSpan.FromHours(24);
}
