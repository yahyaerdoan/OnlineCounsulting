using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.Categories.Domain;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Categories.Infrastructure.Persistence;

public class CategoriesDbContext(DbContextOptions<CategoriesDbContext> options, ITenantProvider tenantProvider) : DbContext(options)
{
    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Categories");

        modelBuilder.Entity<Category>(builder =>
        {
            builder.Property(c => c.Title).HasMaxLength(200).IsRequired();
            builder.Property(c => c.Description).HasMaxLength(2000).IsRequired();
            builder.Property(c => c.Icon).HasMaxLength(2000).IsRequired();
            builder.Property(c => c.IconColor).HasMaxLength(7);
            builder.Property(c => c.RowVersion).IsRowVersion();
            builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
        });

        base.OnModelCreating(modelBuilder);
    }
}
