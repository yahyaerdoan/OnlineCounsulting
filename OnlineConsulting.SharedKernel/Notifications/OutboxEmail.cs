using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.SharedKernel.Notifications;

// Transactional outbox: a business handler enqueues this via DbContextOutboxExtensions.EnqueueEmail
// on its OWN DbContext, in the same unit of work as the business write it belongs to. Because every
// module's DbContext that opts in maps this entity to the SAME physical table (see
// OutboxEmailModelBuilderExtensions - schema/table name are fixed, not module-specific), the row
// commits atomically with whatever business change caused it - a crash or lost connection between
// "business row committed" and "email actually sent" can't lose the email, since the intent to
// send was already durable before any network call to an SMTP server was attempted. A separate
// background dispatcher (OnlineConsulting.Notifications) polls Pending rows and does the actual
// send, with its own retry/backoff - that step is allowed to fail and retry freely because nothing
// about it is transactional with the business write anymore.
//
// No query filter (tenant or soft-delete) is applied to this entity anywhere - the dispatcher has
// no "current tenant" (it's a background service, not a request), so it must see every tenant's
// pending rows. TenantId is still recorded (auto-stamped by TenantSaveChangesInterceptor, same as
// any other ITenantEntity) purely for reporting/debugging, not for access control.
public class OutboxEmail : TenantEntity<Guid>
{
    public required string To { get; set; }
    public string? Cc { get; set; }
    public required string Subject { get; set; }
    public required string HtmlBody { get; set; }

    // Free-form trace back to whatever business action caused this row to be enqueued, e.g.
    // "Message:3fa8...". Not a foreign key (the source could be any module) - purely for
    // support/debugging when a row shows up Failed and someone needs to know what triggered it.
    public string? SourceReference { get; set; }

    public OutboxEmailStatus Status { get; set; } = OutboxEmailStatus.Pending;
    public int Attempts { get; set; }
    public DateTimeOffset NextAttemptAt { get; set; } = DateTimeOffset.UtcNow;
    public string? LastError { get; set; }
    public DateTimeOffset? SentAt { get; set; }
}
