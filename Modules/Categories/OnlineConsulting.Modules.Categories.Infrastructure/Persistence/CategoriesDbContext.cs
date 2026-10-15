using Core.PersistenceLayer.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.Categories.Domain;

namespace OnlineConsulting.Modules.Categories.Infrastructure.Persistence;

public class CategoriesDbContext(DbContextOptions<CategoriesDbContext> options, ITenantContext tenantContext) : TenantDbContext(options, tenantContext)
{
    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _ = modelBuilder.HasDefaultSchema("Categories");

        _ = modelBuilder.Entity<Category>(builder =>
        {
            _ = builder.Property(c => c.Title).HasMaxLength(200).IsRequired();
            _ = builder.Property(c => c.Description).HasMaxLength(2000).IsRequired();
            _ = builder.Property(c => c.Icon).HasMaxLength(2000).IsRequired();
            _ = builder.Property(c => c.IconColor).HasMaxLength(7);
            _ = builder.Property(c => c.RowVersion).IsRowVersion();
        });

        base.OnModelCreating(modelBuilder);
    }
}
