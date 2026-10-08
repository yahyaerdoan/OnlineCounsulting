using OnlineConsulting.Modules.Commerce.Infrastructure.Persistence;
using OnlineConsulting.SharedKernel.Notifications;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Commerce.Infrastructure.Notifications;

public class EmailOutboxWriter(CommerceDbContext context, ITenantProvider tenantProvider) : IEmailOutboxWriter<ICommerceOutboxModule>
{
    public async Task EnqueueAsync(string to, string subject, string htmlBody, string? cc = null, string? sourceReference = null, CancellationToken cancellationToken = default)
    {
        context.EnqueueEmail(tenantProvider.TenantId, to, subject, htmlBody, cc, sourceReference);

        _ = await context.SaveChangesAsync(cancellationToken);
    }
}
