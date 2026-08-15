using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.SharedKernel.Notifications;

/// <summary>Transactional outbox row for an email to be sent by the background dispatcher.</summary>
public class OutboxEmail : TenantEntity<Guid>
{
    public required string To { get; set; }
    public string? Cc { get; set; }
    public required string Subject { get; set; }
    public required string HtmlBody { get; set; }

    /// <summary>Free-form trace to the business action that enqueued this row, e.g. "Message:3fa8...".</summary>
    public string? SourceReference { get; set; }

    public OutboxEmailStatus Status { get; set; } = OutboxEmailStatus.Pending;
    public int Attempts { get; set; }
    public DateTimeOffset NextAttemptAt { get; set; } = DateTimeOffset.UtcNow;
    public string? LastError { get; set; }
    public DateTimeOffset? SentAt { get; set; }
}
