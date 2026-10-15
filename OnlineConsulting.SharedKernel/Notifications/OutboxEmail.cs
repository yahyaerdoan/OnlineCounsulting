using Core.PersistenceLayer.Repositories.Entities;

namespace OnlineConsulting.SharedKernel.Notifications;

/// <summary>Transactional outbox row for an email to be sent by the background dispatcher. Not an ITenantEntity on purpose: the dispatcher reads
/// every tenant's rows, and TenantId only says whose brand the email goes out under.</summary>
public class OutboxEmail : Entity<Guid>
{
    /// <summary>The tenant the email is sent for; set explicitly by EnqueueEmail.</summary>
    public Guid TenantId { get; set; }

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
