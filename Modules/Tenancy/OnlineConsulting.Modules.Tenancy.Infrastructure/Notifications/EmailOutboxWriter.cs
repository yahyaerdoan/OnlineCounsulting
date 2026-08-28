using OnlineConsulting.Modules.Tenancy.Infrastructure.Persistence;
using OnlineConsulting.SharedKernel.Notifications;

namespace OnlineConsulting.Modules.Tenancy.Infrastructure.Notifications;

public class EmailOutboxWriter(TenancyDbContext context) : IEmailOutboxWriter<ITenancyOutboxModule>
{
    public void Enqueue(string to, string subject, string htmlBody, string? cc = null, string? sourceReference = null) =>
        context.EnqueueEmail(to, subject, htmlBody, cc, sourceReference);
}
