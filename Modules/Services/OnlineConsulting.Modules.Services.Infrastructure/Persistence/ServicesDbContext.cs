using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.Services.Domain;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Services.Infrastructure.Persistence;

public class ServicesDbContext(DbContextOptions<ServicesDbContext> options, ITenantProvider tenantProvider) : DbContext(options)
{
    public DbSet<Service> Services => Set<Service>();
    public DbSet<ServiceMediaItem> ServiceMediaItems => Set<ServiceMediaItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _ = modelBuilder.HasDefaultSchema("Services");

        _ = modelBuilder.Entity<Service>(builder =>
        {
            _ = builder.Property(s => s.Title).HasMaxLength(200).IsRequired();
            _ = builder.Property(s => s.Slug).HasMaxLength(220).IsRequired();
            _ = builder.Property(s => s.Description).HasMaxLength(2000).IsRequired();
            _ = builder.Property(s => s.DetailedDescription).HasMaxLength(4000).IsRequired();
            _ = builder.Property(s => s.Price).HasColumnType("decimal(18,2)");
            _ = builder.Property(s => s.PriceType).HasMaxLength(20).IsRequired();
            _ = builder.Property(s => s.PriceMax).HasColumnType("decimal(18,2)");
            _ = builder.Property(s => s.DiscountedPrice).HasColumnType("decimal(18,2)");
            _ = builder.Property(s => s.RowVersion).IsRowVersion();
            _ = builder.HasIndex(s => s.CategoryId);
            _ = builder.HasIndex(s => new { s.TenantId, s.Slug }).IsUnique().HasFilter("[DeletedDate] IS NULL");
            _ = builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
        });

        _ = modelBuilder.Entity<ServiceMediaItem>(builder =>
        {
            _ = builder.Property(m => m.RowVersion).IsRowVersion();
            _ = builder.HasIndex(m => m.ServiceId);
            _ = builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
        });

        base.OnModelCreating(modelBuilder);
    }
}
