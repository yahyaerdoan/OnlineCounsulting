using Microsoft.EntityFrameworkCore;
using OnlineConsulting.SharedKernel.Notifications;

namespace OnlineConsulting.Notifications.Persistence;

// Used exclusively by the dispatcher to poll/update outbox rows - business modules never
// reference this DbContext, they enqueue through their OWN DbContext (see
// DbContextOutboxExtensions.EnqueueEmail) so the write is atomic with their own transaction. This
// context maps the identical physical table (schema/name fixed in
// OutboxEmailModelBuilderExtensions), so both sides agree on where the data lives without
// depending on each other's assemblies.
public class NotificationsDbContext(DbContextOptions<NotificationsDbContext> options) : DbContext(options)
{
    public DbSet<OutboxEmail> OutboxEmails => Set<OutboxEmail>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ConfigureOutboxEmail();

        base.OnModelCreating(modelBuilder);
    }
}
