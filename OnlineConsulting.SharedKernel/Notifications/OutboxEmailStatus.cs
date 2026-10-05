namespace OnlineConsulting.SharedKernel.Notifications;

/// <summary>Pending until sent; Failed once OutboxDispatcherOptions.MaxAttempts is used up.</summary>
public enum OutboxEmailStatus
{
    Pending,
    Sent,
    Failed,
}
