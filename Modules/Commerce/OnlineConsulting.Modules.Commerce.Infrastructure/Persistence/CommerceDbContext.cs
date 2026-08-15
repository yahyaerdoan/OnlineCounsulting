using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.Commerce.Domain;
using OnlineConsulting.SharedKernel.Notifications;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Commerce.Infrastructure.Persistence;

public class CommerceDbContext(DbContextOptions<CommerceDbContext> options, ITenantProvider tenantProvider) : DbContext(options)
{
    public DbSet<UserAddress> UserAddresses => Set<UserAddress>();
    public DbSet<Basket> Baskets => Set<Basket>();
    public DbSet<BasketItem> BasketItems => Set<BasketItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<OutboxEmail> OutboxEmails => Set<OutboxEmail>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Commerce");

        modelBuilder.Entity<UserAddress>(builder =>
        {
            builder.Property(a => a.AddressName).HasMaxLength(200).IsRequired();
            builder.Property(a => a.CompanyName).HasMaxLength(200);
            builder.Property(a => a.Country).HasMaxLength(100).IsRequired();
            builder.Property(a => a.AddressLine).HasMaxLength(500).IsRequired();
            builder.Property(a => a.City).HasMaxLength(100).IsRequired();
            builder.Property(a => a.State).HasMaxLength(100).IsRequired();
            builder.Property(a => a.Zipcode).HasMaxLength(20).IsRequired();
            builder.Property(a => a.Notes).HasMaxLength(1000);
            builder.Property(a => a.RowVersion).IsRowVersion();
            builder.HasIndex(a => a.UserId);
            builder.HasQueryFilter(a => a.TenantId == tenantProvider.TenantId && a.DeletedDate == null);
        });

        modelBuilder.Entity<Basket>(builder =>
        {
            builder.Property(b => b.SubTotalPrice).HasColumnType("decimal(18,2)");
            builder.Property(b => b.TotalPrice).HasColumnType("decimal(18,2)");
            builder.Property(b => b.RowVersion).IsRowVersion();
            builder.HasIndex(b => b.UserId);
            builder.HasIndex(b => b.GuestId);
            builder.HasQueryFilter(b => b.TenantId == tenantProvider.TenantId && b.DeletedDate == null);
        });

        modelBuilder.Entity<BasketItem>(builder =>
        {
            builder.Property(i => i.Price).HasColumnType("decimal(18,2)");
            builder.Property(i => i.TaxAmount).HasColumnType("decimal(18,2)");
            builder.Property(i => i.SubTotalPrice).HasColumnType("decimal(18,2)");
            builder.Property(i => i.TotalPrice).HasColumnType("decimal(18,2)");
            builder.Property(i => i.RowVersion).IsRowVersion();
            builder.HasIndex(i => i.BasketId);
            builder.HasQueryFilter(i => i.TenantId == tenantProvider.TenantId && i.DeletedDate == null);
        });

        modelBuilder.Entity<Order>(builder =>
        {
            builder.Property(o => o.OrderNumber).HasMaxLength(50).IsRequired();
            builder.Property(o => o.OrderStatus).HasMaxLength(50).IsRequired();
            builder.Property(o => o.PaymentStatus).HasMaxLength(50).IsRequired();
            builder.Property(o => o.PaymentProvider).HasMaxLength(50);
            builder.Property(o => o.ProviderPaymentId).HasMaxLength(200);
            builder.Property(o => o.RowVersion).IsRowVersion();
            builder.HasIndex(o => o.UserId);
            builder.HasQueryFilter(o => o.TenantId == tenantProvider.TenantId && o.DeletedDate == null);
        });

        modelBuilder.Entity<OrderItem>(builder =>
        {
            builder.Property(i => i.UnitPrice).HasColumnType("decimal(18,2)");
            builder.Property(i => i.TaxAmount).HasColumnType("decimal(18,2)");
            builder.Property(i => i.SubTotalPrice).HasColumnType("decimal(18,2)");
            builder.Property(i => i.TotalPrice).HasColumnType("decimal(18,2)");
            builder.Property(i => i.RowVersion).IsRowVersion();
            builder.HasIndex(i => i.OrderId);
            builder.HasQueryFilter(i => i.TenantId == tenantProvider.TenantId && i.DeletedDate == null);
        });

        modelBuilder.ConfigureOutboxEmail();

        base.OnModelCreating(modelBuilder);
    }
}
