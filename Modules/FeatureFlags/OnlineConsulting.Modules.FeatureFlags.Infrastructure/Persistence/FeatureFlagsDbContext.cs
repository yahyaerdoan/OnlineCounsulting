using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.FeatureFlags.Domain;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.FeatureFlags.Infrastructure.Persistence;

public class FeatureFlagsDbContext(DbContextOptions<FeatureFlagsDbContext> options, ITenantProvider tenantProvider) : DbContext(options)
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
            _ = builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
        });

        base.OnModelCreating(modelBuilder);
    }
}
