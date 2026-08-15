using Microsoft.EntityFrameworkCore;
using OnlineConsulting.SharedKernel.Notifications;

namespace OnlineConsulting.Notifications.Persistence;

/// <summary>Used exclusively by the dispatcher to poll/update outbox rows; business modules enqueue through their own DbContext instead.</summary>
public class NotificationsDbContext(DbContextOptions<NotificationsDbContext> options) : DbContext(options)
{
    public DbSet<OutboxEmail> OutboxEmails => Set<OutboxEmail>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ConfigureOutboxEmail();

        base.OnModelCreating(modelBuilder);
    }
}
