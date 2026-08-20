using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.Equipment.Domain;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Equipment.Infrastructure.Persistence;

public class EquipmentDbContext(DbContextOptions<EquipmentDbContext> options, ITenantProvider tenantProvider) : DbContext(options)
{
    public DbSet<EquipmentItem> EquipmentItems => Set<EquipmentItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _ = modelBuilder.HasDefaultSchema("Equipment");

        _ = modelBuilder.Entity<EquipmentItem>(builder =>
        {
            _ = builder.Property(e => e.Type).HasMaxLength(100).IsRequired();
            _ = builder.Property(e => e.Brand).HasMaxLength(100);
            _ = builder.Property(e => e.Model).HasMaxLength(100);
            _ = builder.Property(e => e.SerialNumber).HasMaxLength(100);
            _ = builder.Property(e => e.Notes).HasMaxLength(2000);
            _ = builder.Property(e => e.RowVersion).IsRowVersion();
            _ = builder.HasIndex(e => e.UserId);
            _ = builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
        });

        base.OnModelCreating(modelBuilder);
    }
}
