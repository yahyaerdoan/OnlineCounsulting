using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.Services.Domain;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Services.Infrastructure.Persistence;

public class ServicesDbContext(DbContextOptions<ServicesDbContext> options, ITenantProvider tenantProvider) : DbContext(options)
{
    public DbSet<Service> Services => Set<Service>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Services");

        modelBuilder.Entity<Service>(builder =>
        {
            builder.Property(s => s.Title).HasMaxLength(200).IsRequired();
            builder.Property(s => s.Slug).HasMaxLength(220).IsRequired();
            builder.Property(s => s.Description).HasMaxLength(2000).IsRequired();
            builder.Property(s => s.DetailedDescription).HasMaxLength(4000).IsRequired();
            builder.Property(s => s.Price).HasColumnType("decimal(18,2)");
            builder.Property(s => s.DiscountedPrice).HasColumnType("decimal(18,2)");
            builder.Property(s => s.RowVersion).IsRowVersion();
            builder.HasIndex(s => s.CategoryId);
            builder.HasIndex(s => new { s.TenantId, s.Slug }).IsUnique().HasFilter("[DeletedDate] IS NULL");
            builder.HasQueryFilter(s => s.TenantId == tenantProvider.TenantId && s.DeletedDate == null);
        });

        base.OnModelCreating(modelBuilder);
    }
}
