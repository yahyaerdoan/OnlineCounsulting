using Microsoft.EntityFrameworkCore;

namespace OnlineConsulting.SharedKernel.Notifications;

public static class DbContextOutboxExtensions
{
    /// <summary>Stages an OutboxEmail row on the given context without saving; caller's own SaveChanges commits it.</summary>
    public static void EnqueueEmail(this DbContext context, string to, string subject, string htmlBody, string? cc = null, string? sourceReference = null)
    {
        _ = context.Set<OutboxEmail>().Add(new OutboxEmail
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
