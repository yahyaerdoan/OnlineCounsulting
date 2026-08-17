using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.Scheduling.Domain;
using OnlineConsulting.SharedKernel.Notifications;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Scheduling.Infrastructure.Persistence;

public class SchedulingDbContext(DbContextOptions<SchedulingDbContext> options, ITenantProvider tenantProvider) : DbContext(options)
{
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<AvailabilityRule> AvailabilityRules => Set<AvailabilityRule>();
    public DbSet<OutboxEmail> OutboxEmails => Set<OutboxEmail>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Scheduling");

        modelBuilder.Entity<Appointment>(builder =>
        {
            builder.Property(a => a.Status).HasMaxLength(30).IsRequired();
            builder.Property(a => a.CustomerNote).HasMaxLength(1000);
            builder.Property(a => a.RowVersion).IsRowVersion();
            builder.HasIndex(a => a.UserId);
            builder.HasIndex(a => a.ServiceId);
            builder.HasIndex(a => new { a.TenantId, a.ScheduledStart });
            builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
        });

        modelBuilder.Entity<AvailabilityRule>(builder =>
        {
            builder.Property(r => r.RowVersion).IsRowVersion();
            builder.HasIndex(r => new { r.TenantId, r.DayOfWeek });
            builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
        });

        modelBuilder.ConfigureOutboxEmail();

        base.OnModelCreating(modelBuilder);
    }
}
