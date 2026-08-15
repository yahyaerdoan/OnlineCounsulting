using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.Media.Domain;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Media.Infrastructure.Persistence;

public class MediaDbContext(DbContextOptions<MediaDbContext> options, ITenantProvider tenantProvider) : DbContext(options)
{
    public DbSet<MediaAsset> MediaAssets => Set<MediaAsset>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Media");

        modelBuilder.Entity<MediaAsset>(builder =>
        {
            builder.Property(x => x.Url).HasMaxLength(500).IsRequired();
            builder.Property(x => x.AltText).HasMaxLength(300);
            builder.Property(x => x.ContentType).HasMaxLength(100).IsRequired();
            builder.Property(x => x.StorageProvider).HasMaxLength(50).IsRequired();
            builder.Property(x => x.RowVersion).IsRowVersion();
            builder.HasQueryFilter(x => x.TenantId == tenantProvider.TenantId && x.DeletedDate == null);
        });

        base.OnModelCreating(modelBuilder);
    }
}
