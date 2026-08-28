using OnlineConsulting.Modules.Scheduling.Infrastructure.Persistence;
using OnlineConsulting.SharedKernel.Notifications;

namespace OnlineConsulting.Modules.Scheduling.Infrastructure.Notifications;

public class EmailOutboxWriter(SchedulingDbContext context) : IEmailOutboxWriter<ISchedulingOutboxModule>
{
    public async Task EnqueueAsync(string to, string subject, string htmlBody, string? cc = null, string? sourceReference = null, CancellationToken cancellationToken = default)
    {
        context.EnqueueEmail(to, subject, htmlBody, cc, sourceReference);
        _ = await context.SaveChangesAsync(cancellationToken);
    }
}
