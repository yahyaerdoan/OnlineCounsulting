using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.Media.Domain;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Media.Infrastructure.Persistence;

public class MediaDbContext(DbContextOptions<MediaDbContext> options, ITenantProvider tenantProvider) : DbContext(options), ITenantScopedDbContext
{
    public Guid CurrentTenantId => tenantProvider.TenantId;

    public DbSet<MediaAsset> MediaAssets => Set<MediaAsset>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _ = modelBuilder.HasDefaultSchema("Media");

        _ = modelBuilder.Entity<MediaAsset>(builder =>
        {
            _ = builder.Property(x => x.Url).HasMaxLength(500).IsRequired();
            _ = builder.Property(x => x.AltText).HasMaxLength(300);
            _ = builder.Property(x => x.ContentType).HasMaxLength(100).IsRequired();
            _ = builder.Property(x => x.StorageProvider).HasMaxLength(50).IsRequired();
            _ = builder.Property(x => x.RowVersion).IsRowVersion();
            _ = builder.ApplyTenantAndSoftDeleteFilter(this);
        });

        base.OnModelCreating(modelBuilder);
    }
}
