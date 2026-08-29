using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.Scheduling.Domain;
using OnlineConsulting.SharedKernel.Notifications;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Scheduling.Infrastructure.Persistence;

public class SchedulingDbContext(DbContextOptions<SchedulingDbContext> options, ITenantProvider tenantProvider) : DbContext(options)
{
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<AvailabilityRule> AvailabilityRules => Set<AvailabilityRule>();
    public DbSet<WorkOrder> WorkOrders => Set<WorkOrder>();
    public DbSet<WorkOrderMediaItem> WorkOrderMediaItems => Set<WorkOrderMediaItem>();
    public DbSet<AppointmentMediaItem> AppointmentMediaItems => Set<AppointmentMediaItem>();
    public DbSet<OutboxEmail> OutboxEmails => Set<OutboxEmail>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _ = modelBuilder.HasDefaultSchema("Scheduling");

        _ = modelBuilder.Entity<Appointment>(builder =>
        {
            _ = builder.Property(a => a.Status).HasMaxLength(30).IsRequired();
            _ = builder.Property(a => a.CustomerNote).HasMaxLength(1000);
            _ = builder.Property(a => a.ServiceAddress).HasMaxLength(500);
            _ = builder.Property(a => a.RowVersion).IsRowVersion();
            _ = builder.HasIndex(a => a.UserId);
            _ = builder.HasIndex(a => a.ServiceId);
            _ = builder.HasIndex(a => a.AssignedTechnicianUserId);
            _ = builder.HasIndex(a => new { a.TenantId, a.ScheduledStart });
            _ = builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
        });

        _ = modelBuilder.Entity<AvailabilityRule>(builder =>
        {
            _ = builder.Property(r => r.RowVersion).IsRowVersion();
            _ = builder.HasIndex(r => new { r.TenantId, r.DayOfWeek });
            _ = builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
        });

        _ = modelBuilder.Entity<WorkOrder>(builder =>
        {
            _ = builder.Property(w => w.PartsUsed).HasMaxLength(2000);
            _ = builder.Property(w => w.TechnicianNotes).HasMaxLength(2000);
            _ = builder.Property(w => w.RowVersion).IsRowVersion();
            _ = builder.HasIndex(w => w.AppointmentId).IsUnique();
            _ = builder.HasIndex(w => w.TechnicianUserId);
            _ = builder.HasIndex(w => w.EquipmentId);
            _ = builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
        });

        _ = modelBuilder.Entity<WorkOrderMediaItem>(builder =>
        {
            _ = builder.Property(m => m.RowVersion).IsRowVersion();
            _ = builder.HasIndex(m => m.WorkOrderId);
            _ = builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
        });

        _ = modelBuilder.Entity<AppointmentMediaItem>(builder =>
        {
            _ = builder.Property(m => m.RowVersion).IsRowVersion();
            _ = builder.HasIndex(m => m.AppointmentId);
            _ = builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
        });

        modelBuilder.ConfigureOutboxEmail(ownsMigration: false);

        base.OnModelCreating(modelBuilder);
    }
}
