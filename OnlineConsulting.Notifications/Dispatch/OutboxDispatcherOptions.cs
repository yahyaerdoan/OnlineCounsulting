namespace OnlineConsulting.Notifications.Dispatch;

public class OutboxDispatcherOptions
{
    public TimeSpan PollInterval { get; set; } = TimeSpan.FromSeconds(15);
    public int BatchSize { get; set; } = 20;
    public int MaxAttempts { get; set; } = 5;

    /// <summary>Caps the exponential (2^Attempts minutes) backoff between dispatcher attempts so a permanently-broken row doesn't wait days before hitting MaxAttempts.</summary>
    public TimeSpan BackoffCap { get; set; } = TimeSpan.FromHours(1);
}
