using Core.SecurityLayer.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.Identity.Domain;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Identity.Infrastructure.Persistence;

public class AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options) : BaseIdentityDbContext<User, Role, Guid>(options)
{
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Own schema, same approach as CategoriesDbContext.
        modelBuilder.HasDefaultSchema("identity");

        // No tenant onboarding yet - defaults to the placeholder tenant.
        modelBuilder.Entity<User>().Property(u => u.TenantId).HasDefaultValue(TenantDefaults.DefaultTenantId);

        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.Now;
        foreach (var entry in ChangeTracker.Entries<User>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.IsActive = true;
                    entry.Entity.CreatedDate = now;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedDate = now;
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
