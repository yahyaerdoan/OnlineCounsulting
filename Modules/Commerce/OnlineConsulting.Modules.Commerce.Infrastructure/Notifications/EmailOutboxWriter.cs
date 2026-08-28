using OnlineConsulting.Modules.Commerce.Infrastructure.Persistence;
using OnlineConsulting.SharedKernel.Notifications;

namespace OnlineConsulting.Modules.Commerce.Infrastructure.Notifications;

public class EmailOutboxWriter(CommerceDbContext context) : IEmailOutboxWriter<ICommerceOutboxModule>
{
    public async Task EnqueueAsync(string to, string subject, string htmlBody, string? cc = null, string? sourceReference = null, CancellationToken cancellationToken = default)
    {
        context.EnqueueEmail(to, subject, htmlBody, cc, sourceReference);

        _ = await context.SaveChangesAsync(cancellationToken);
    }
}
