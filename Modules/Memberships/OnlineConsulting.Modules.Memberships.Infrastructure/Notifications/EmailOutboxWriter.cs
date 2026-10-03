using OnlineConsulting.Modules.Memberships.Infrastructure.Persistence;
using OnlineConsulting.SharedKernel.Notifications;

namespace OnlineConsulting.Modules.Memberships.Infrastructure.Notifications;

public class EmailOutboxWriter(MembershipsDbContext context) : IEmailOutboxWriter<IMembershipsOutboxModule>
{
    public async Task EnqueueAsync(string to, string subject, string htmlBody, string? cc = null, string? sourceReference = null, CancellationToken cancellationToken = default)
    {
        context.EnqueueEmail(to, subject, htmlBody, cc, sourceReference);
        _ = await context.SaveChangesAsync(cancellationToken);
    }
}
