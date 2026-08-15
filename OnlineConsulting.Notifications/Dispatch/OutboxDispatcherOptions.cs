namespace OnlineConsulting.Notifications.Dispatch;

public class OutboxDispatcherOptions
{
    public TimeSpan PollInterval { get; set; } = TimeSpan.FromSeconds(15);
    public int BatchSize { get; set; } = 20;
    public int MaxAttempts { get; set; } = 5;

    // Backoff between dispatcher attempts (minutes scale) - separate from and much coarser than
    // the in-process Polly retries wrapping a single send call (seconds scale, see
    // OutboxDispatcher). 2^Attempts minutes, capped so a permanently-broken row doesn't push its
    // next attempt out for days before finally hitting MaxAttempts.
    public TimeSpan BackoffCap { get; set; } = TimeSpan.FromHours(1);
}
