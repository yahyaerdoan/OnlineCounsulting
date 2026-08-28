using OnlineConsulting.Modules.Identity.Infrastructure.Persistence;
using OnlineConsulting.SharedKernel.Notifications;

namespace OnlineConsulting.Modules.Identity.Infrastructure.Notifications;

public class EmailOutboxWriter(AppIdentityDbContext context) : IEmailOutboxWriter<IIdentityOutboxModule>
{
    public void Enqueue(string to, string subject, string htmlBody, string? cc = null, string? sourceReference = null) =>
        context.EnqueueEmail(to, subject, htmlBody, cc, sourceReference);
}
