using OnlineConsulting.Modules.Inquiries.Infrastructure.Persistence;
using OnlineConsulting.SharedKernel.Notifications;

namespace OnlineConsulting.Modules.Inquiries.Infrastructure.Notifications;

public class EmailOutboxWriter(InquiriesDbContext context) : IEmailOutboxWriter<IInquiriesOutboxModule>
{
    public async Task EnqueueAsync(string to, string subject, string htmlBody, string? cc = null, string? sourceReference = null, CancellationToken cancellationToken = default)
    {
        context.EnqueueEmail(to, subject, htmlBody, cc, sourceReference);
        _ = await context.SaveChangesAsync(cancellationToken);
    }
}
