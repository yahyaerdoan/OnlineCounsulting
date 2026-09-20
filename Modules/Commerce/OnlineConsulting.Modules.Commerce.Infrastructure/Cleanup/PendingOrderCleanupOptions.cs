namespace OnlineConsulting.Modules.Commerce.Infrastructure.Cleanup;

/// <summary>Config shape mirrors TenancyCleanupOptions (see OnlineConsulting.Modules.Tenancy.Infrastructure.Cleanup.TenancyCleanupOptions) - bound from the Commerce:PendingOrderCleanup config section.</summary>
public class PendingOrderCleanupOptions
{
    public TimeSpan PollInterval { get; set; } = TimeSpan.FromMinutes(15);

    /// <summary>An order younger than this is skipped entirely - the customer may still be mid-checkout (entering card details, resolving 3D Secure), so reconciling this early would just be noise.</summary>
    public TimeSpan ReconcileAfter { get; set; } = TimeSpan.FromMinutes(10);

    /// <summary>How long an order may stay Pending - after both the webhook and the reconciliation check above have had a chance to catch it - before PendingOrderCleanupService gives up and cancels it as an abandoned checkout.</summary>
    public TimeSpan ExpireAfter { get; set; } = TimeSpan.FromHours(24);
}
