using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.Equipment.Domain;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Equipment.Infrastructure.Persistence;

public class EquipmentDbContext(DbContextOptions<EquipmentDbContext> options, ITenantProvider tenantProvider) : DbContext(options)
{
    public DbSet<EquipmentItem> EquipmentItems => Set<EquipmentItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Equipment");

        modelBuilder.Entity<EquipmentItem>(builder =>
        {
            builder.Property(e => e.Type).HasMaxLength(100).IsRequired();
            builder.Property(e => e.Brand).HasMaxLength(100);
            builder.Property(e => e.Model).HasMaxLength(100);
            builder.Property(e => e.SerialNumber).HasMaxLength(100);
            builder.Property(e => e.Notes).HasMaxLength(2000);
            builder.Property(e => e.RowVersion).IsRowVersion();
            builder.HasIndex(e => e.UserId);
            builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
        });

        base.OnModelCreating(modelBuilder);
    }
}
