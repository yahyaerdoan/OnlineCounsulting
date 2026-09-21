namespace OnlineConsulting.Notifications.Dispatch;

public class OutboxDispatcherOptions
{
    public TimeSpan PollInterval { get; set; } = TimeSpan.FromSeconds(15);
    public int BatchSize { get; set; } = 20;
    public int MaxAttempts { get; set; } = 5;

    /// <summary>Caps concurrent SMTP connections, not CPU parallelism - each send opens its own connection.</summary>
    public int MaxConcurrentSends { get; set; } = 5;

    /// <summary>Caps the exponential (2^Attempts minutes) backoff between dispatcher attempts so a permanently-broken row doesn't wait days before hitting MaxAttempts.</summary>
    public TimeSpan BackoffCap { get; set; } = TimeSpan.FromHours(1);
}
