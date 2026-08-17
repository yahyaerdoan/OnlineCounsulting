using OnlineConsulting.Modules.Inquiries.Infrastructure.Persistence;
using OnlineConsulting.SharedKernel.Notifications;

namespace OnlineConsulting.Modules.Inquiries.Infrastructure.Notifications;

public class EmailOutboxWriter(InquiriesDbContext context) : IEmailOutboxWriter
{
    public void Enqueue(string to, string subject, string htmlBody, string? cc = null, string? sourceReference = null) =>
        context.EnqueueEmail(to, subject, htmlBody, cc, sourceReference);
}
