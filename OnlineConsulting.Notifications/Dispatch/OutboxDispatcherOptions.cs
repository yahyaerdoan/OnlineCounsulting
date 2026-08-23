namespace OnlineConsulting.Notifications.Dispatch;

public class OutboxDispatcherOptions
{
    public TimeSpan PollInterval { get; set; } = TimeSpan.FromSeconds(15);
    public int BatchSize { get; set; } = 20;
    public int MaxAttempts { get; set; } = 5;

    /// <summary>Upper bound on how many emails this dispatcher sends at once. Each send opens its
    /// own SMTP connection (MailKitEmailSender), so this is really a cap on concurrent SMTP
    /// connections/throughput against the mail server, not a CPU-bound parallelism knob.</summary>
    public int MaxConcurrentSends { get; set; } = 5;

    /// <summary>Caps the exponential (2^Attempts minutes) backoff between dispatcher attempts so a permanently-broken row doesn't wait days before hitting MaxAttempts.</summary>
    public TimeSpan BackoffCap { get; set; } = TimeSpan.FromHours(1);
}
