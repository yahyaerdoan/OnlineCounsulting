using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using OnlineConsulting.Modules.Tenancy.Domain;
using OnlineConsulting.SharedKernel.Notifications;

namespace OnlineConsulting.Modules.Tenancy.Infrastructure.Persistence;

/// <summary>Not tenant-scoped - Tenant/ModuleOffering/Bundle/TenantSubscription/TenantSubscriptionItem are platform-owner data, not per-tenant data, so this context applies no tenant query filter.</summary>
public class TenancyDbContext(DbContextOptions<TenancyDbContext> options) : DbContext(options)
{
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<ModuleOffering> ModuleOfferings => Set<ModuleOffering>();
    public DbSet<Bundle> Bundles => Set<Bundle>();
    public DbSet<TenantSubscription> TenantSubscriptions => Set<TenantSubscription>();
    public DbSet<TenantSubscriptionItem> TenantSubscriptionItems => Set<TenantSubscriptionItem>();
    public DbSet<OutboxEmail> OutboxEmails => Set<OutboxEmail>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _ = modelBuilder.HasDefaultSchema("Tenancy");

        _ = modelBuilder.Entity<Tenant>(builder =>
        {
            _ = builder.Property(t => t.Name).HasMaxLength(200).IsRequired();
            _ = builder.Property(t => t.Slug).HasMaxLength(200).IsRequired();
            _ = builder.Property(t => t.Status).HasMaxLength(30).IsRequired();
            _ = builder.Property(t => t.PrimaryContactEmail).HasMaxLength(256).IsRequired();
            _ = builder.Property(t => t.ProviderCustomerId).HasMaxLength(100);
            _ = builder.Property(t => t.RowVersion).IsRowVersion();
            _ = builder.HasIndex(t => t.Slug).IsUnique();
        });

        _ = modelBuilder.Entity<ModuleOffering>(builder =>
        {
            _ = builder.Property(m => m.Key).HasMaxLength(100).IsRequired();
            _ = builder.Property(m => m.Name).HasMaxLength(200).IsRequired();
            _ = builder.Property(m => m.BillingCycle).HasMaxLength(30).IsRequired();
            _ = builder.Property(m => m.Price).HasColumnType("decimal(18,2)");
            _ = builder.Property(m => m.ProviderProductId).HasMaxLength(100);
            _ = builder.Property(m => m.ProviderPriceId).HasMaxLength(100);
            _ = builder.Property(m => m.RowVersion).IsRowVersion();
            _ = builder.HasIndex(m => m.Key).IsUnique();
        });

        _ = modelBuilder.Entity<Bundle>(builder =>
        {
            _ = builder.Property(b => b.Name).HasMaxLength(200).IsRequired();
            _ = builder.Property(b => b.RowVersion).IsRowVersion();

            builder.Property(b => b.ModuleKeys)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Length == 0 ? new List<string>() : v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
                .Metadata.SetValueComparer(new ValueComparer<List<string>>(
                    (a, b) => a != null && b != null && a.SequenceEqual(b),
                    v => v.Aggregate(0, (hash, s) => HashCode.Combine(hash, s.GetHashCode())),
                    v => v.ToList()));
        });

        _ = modelBuilder.Entity<TenantSubscription>(builder =>
        {
            _ = builder.Property(s => s.Status).HasMaxLength(30).IsRequired();
            _ = builder.Property(s => s.ProviderSubscriptionId).HasMaxLength(100);
            _ = builder.Property(s => s.RowVersion).IsRowVersion();
            _ = builder.HasIndex(s => s.TenantId);
        });

        _ = modelBuilder.Entity<TenantSubscriptionItem>(builder =>
        {
            _ = builder.Property(i => i.ModuleKey).HasMaxLength(100).IsRequired();
            _ = builder.Property(i => i.Status).HasMaxLength(30).IsRequired().HasDefaultValue(TenantSubscriptionItemStatuses.Active);
            _ = builder.Property(i => i.ProviderSubscriptionItemId).HasMaxLength(100);
            _ = builder.Property(i => i.PriceAtAddition).HasColumnType("decimal(18,2)");
            _ = builder.Property(i => i.RowVersion).IsRowVersion();
            _ = builder.HasIndex(i => i.TenantSubscriptionId);
        });

        modelBuilder.ConfigureOutboxEmail(ownsMigration: false);

        base.OnModelCreating(modelBuilder);
    }
}
