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
        modelBuilder.HasDefaultSchema("Identity");

        modelBuilder.Entity<User>(builder =>
        {
            builder.Property(u => u.TenantId).HasDefaultValue(TenantDefaults.DefaultTenantId);
            builder.Property(u => u.CreatedDate).HasConversion(DateTimeOffsetConverters.NonNullable);
            builder.Property(u => u.UpdatedDate).HasConversion(DateTimeOffsetConverters.Nullable);
            builder.Property(u => u.DeletedDate).HasConversion(DateTimeOffsetConverters.Nullable);
        });

        modelBuilder.Entity<DeviceToken>(builder =>
        {
            builder.Property(d => d.Token).HasMaxLength(500).IsRequired();
            builder.Property(d => d.Platform).HasMaxLength(20).IsRequired();
            builder.HasIndex(d => d.Token).IsUnique();
            builder.HasIndex(d => d.UserId);
        });

        modelBuilder.Entity<Invite>(builder =>
        {
            builder.Property(i => i.Email).HasMaxLength(256).IsRequired();
            builder.Property(i => i.Token).HasMaxLength(200).IsRequired();
            builder.Property(i => i.RoleName).HasMaxLength(256).IsRequired();
            builder.Property(i => i.Status).HasMaxLength(20).IsRequired();
            builder.HasIndex(i => i.Token).IsUnique();
            builder.HasIndex(i => new { i.TenantId, i.Email });
        });

        modelBuilder.ConfigureOutboxEmail();

        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<User>().Where(e => e.State == EntityState.Added))
            entry.Entity.IsActive = true;

        return base.SaveChangesAsync(cancellationToken);
    }
}
