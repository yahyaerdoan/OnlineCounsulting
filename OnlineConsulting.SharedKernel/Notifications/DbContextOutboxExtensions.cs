using Microsoft.EntityFrameworkCore;

namespace OnlineConsulting.SharedKernel.Notifications;

public static class DbContextOutboxExtensions
{
    // Deliberately synchronous and non-saving - this only stages the row via ChangeTracker.Add.
    // The caller's own SaveChangesAsync (already happening inside its transaction, e.g. via
    // EfTransactionAddingBehavior) is what actually commits it, atomically with whatever business
    // rows that same handler wrote. Calling SaveChanges here would defeat the entire point.
    public static void EnqueueEmail(this DbContext context, string to, string subject, string htmlBody, string? cc = null, string? sourceReference = null)
    {
        context.Set<OutboxEmail>().Add(new OutboxEmail
        {
            Id = Guid.NewGuid(),
            To = to,
            Cc = cc,
            Subject = subject,
            HtmlBody = htmlBody,
            SourceReference = sourceReference,
        });
    }
}
