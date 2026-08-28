using OnlineConsulting.Modules.Commerce.Infrastructure.Persistence;
using OnlineConsulting.SharedKernel.Notifications;

namespace OnlineConsulting.Modules.Commerce.Infrastructure.Notifications;

public class EmailOutboxWriter(CommerceDbContext context) : IEmailOutboxWriter<ICommerceOutboxModule>
{
    public void Enqueue(string to, string subject, string htmlBody, string? cc = null, string? sourceReference = null) =>
        context.EnqueueEmail(to, subject, htmlBody, cc, sourceReference);
}
