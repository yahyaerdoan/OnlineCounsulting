using Core.SecurityLayer.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.Identity.Domain;
using OnlineConsulting.SharedKernel.Notifications;
using OnlineConsulting.SharedKernel.Persistence;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Identity.Infrastructure.Persistence;

public class AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options) : BaseIdentityDbContext<User, Role, Guid>(options)
{
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<DeviceToken> DeviceTokens => Set<DeviceToken>();
    public DbSet<OutboxEmail> OutboxEmails => Set<OutboxEmail>();
    public DbSet<Invite> Invites => Set<Invite>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _ = modelBuilder.HasDefaultSchema("Identity");

        _ = modelBuilder.Entity<User>(builder =>
        {
            _ = builder.Property(u => u.TenantId).HasDefaultValue(TenantDefaults.DefaultTenantId);
            _ = builder.Property(u => u.CreatedDate).HasConversion(DateTimeOffsetConverters.NonNullable);
            _ = builder.Property(u => u.UpdatedDate).HasConversion(DateTimeOffsetConverters.Nullable);
            _ = builder.Property(u => u.DeletedDate).HasConversion(DateTimeOffsetConverters.Nullable);
        });

        _ = modelBuilder.Entity<DeviceToken>(builder =>
        {
            _ = builder.Property(d => d.Token).HasMaxLength(500).IsRequired();
            _ = builder.Property(d => d.Platform).HasMaxLength(20).IsRequired();
            _ = builder.HasIndex(d => d.Token).IsUnique();
            _ = builder.HasIndex(d => d.UserId);
        });

        _ = modelBuilder.Entity<Invite>(builder =>
        {
            _ = builder.Property(i => i.Email).HasMaxLength(256).IsRequired();
            _ = builder.Property(i => i.Token).HasMaxLength(200).IsRequired();
            _ = builder.Property(i => i.RoleName).HasMaxLength(256).IsRequired();
            _ = builder.Property(i => i.Status).HasMaxLength(20).IsRequired();
            _ = builder.HasIndex(i => i.Token).IsUnique();
            _ = builder.HasIndex(i => new { i.TenantId, i.Email });
        });

        modelBuilder.ConfigureOutboxEmail(ownsMigration: false);

        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<User>().Where(e => e.State == EntityState.Added))
        {
            entry.Entity.IsActive = true;
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
