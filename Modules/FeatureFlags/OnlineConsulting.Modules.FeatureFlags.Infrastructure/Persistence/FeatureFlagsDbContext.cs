using Core.PersistenceLayer.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.FeatureFlags.Domain;

namespace OnlineConsulting.Modules.FeatureFlags.Infrastructure.Persistence;

public class FeatureFlagsDbContext(DbContextOptions<FeatureFlagsDbContext> options, ITenantContext tenantContext) : TenantDbContext(options, tenantContext)
{
    public DbSet<FeatureFlag> FeatureFlags => Set<FeatureFlag>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _ = modelBuilder.HasDefaultSchema("FeatureFlags");

        _ = modelBuilder.Entity<FeatureFlag>(builder =>
        {
            _ = builder.Property(f => f.Key).HasMaxLength(200).IsRequired();
            _ = builder.HasIndex(f => new { f.TenantId, f.Key }).IsUnique();
            _ = builder.Property(f => f.RowVersion).IsRowVersion();
        });

        base.OnModelCreating(modelBuilder);
    }
}
