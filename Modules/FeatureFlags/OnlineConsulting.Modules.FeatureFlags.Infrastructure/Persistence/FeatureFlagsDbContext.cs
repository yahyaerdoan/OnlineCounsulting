using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.FeatureFlags.Domain;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.FeatureFlags.Infrastructure.Persistence;

public class FeatureFlagsDbContext(DbContextOptions<FeatureFlagsDbContext> options, ITenantProvider tenantProvider) : DbContext(options)
{
    public DbSet<FeatureFlag> FeatureFlags => Set<FeatureFlag>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("FeatureFlags");

        modelBuilder.Entity<FeatureFlag>(builder =>
        {
            builder.Property(f => f.Key).HasMaxLength(200).IsRequired();
            builder.HasIndex(f => new { f.TenantId, f.Key }).IsUnique();
            builder.Property(f => f.RowVersion).IsRowVersion();
            builder.HasQueryFilter(f => f.TenantId == tenantProvider.TenantId && f.DeletedDate == null);
        });

        base.OnModelCreating(modelBuilder);
    }
}
