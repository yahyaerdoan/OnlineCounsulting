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
        _ = modelBuilder.HasDefaultSchema("Commerce");

        _ = modelBuilder.Entity<UserAddress>(builder =>
        {
            _ = builder.Property(a => a.AddressName).HasMaxLength(200).IsRequired();
            _ = builder.Property(a => a.CompanyName).HasMaxLength(200);
            _ = builder.Property(a => a.Country).HasMaxLength(100).IsRequired();
            _ = builder.Property(a => a.AddressLine).HasMaxLength(500).IsRequired();
            _ = builder.Property(a => a.City).HasMaxLength(100).IsRequired();
            _ = builder.Property(a => a.State).HasMaxLength(100).IsRequired();
            _ = builder.Property(a => a.Zipcode).HasMaxLength(20).IsRequired();
            _ = builder.Property(a => a.Notes).HasMaxLength(1000);
            _ = builder.Property(a => a.RowVersion).IsRowVersion();
            _ = builder.HasIndex(a => a.UserId);
            _ = builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
        });

        _ = modelBuilder.Entity<Basket>(builder =>
        {
            _ = builder.Property(b => b.SubTotalPrice).HasColumnType("decimal(18,2)");
            _ = builder.Property(b => b.TotalPrice).HasColumnType("decimal(18,2)");
            _ = builder.Property(b => b.RowVersion).IsRowVersion();
            _ = builder.HasIndex(b => b.UserId);
            _ = builder.HasIndex(b => b.GuestId);
            _ = builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
        });

        _ = modelBuilder.Entity<BasketItem>(builder =>
        {
            _ = builder.Property(i => i.Price).HasColumnType("decimal(18,2)");
            _ = builder.Property(i => i.TaxAmount).HasColumnType("decimal(18,2)");
            _ = builder.Property(i => i.SubTotalPrice).HasColumnType("decimal(18,2)");
            _ = builder.Property(i => i.TotalPrice).HasColumnType("decimal(18,2)");
            _ = builder.Property(i => i.RowVersion).IsRowVersion();
            _ = builder.HasIndex(i => i.BasketId);
            _ = builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
        });

        _ = modelBuilder.Entity<Order>(builder =>
        {
            _ = builder.Property(o => o.OrderNumber).HasMaxLength(50).IsRequired();
            _ = builder.Property(o => o.OrderStatus).HasMaxLength(50).IsRequired();
            _ = builder.Property(o => o.PaymentStatus).HasMaxLength(50).IsRequired();
            _ = builder.Property(o => o.PaymentProvider).HasMaxLength(50);
            _ = builder.Property(o => o.ProviderPaymentId).HasMaxLength(200);
            _ = builder.Property(o => o.RowVersion).IsRowVersion();
            _ = builder.HasIndex(o => o.UserId);
            _ = builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
        });

        _ = modelBuilder.Entity<OrderItem>(builder =>
        {
            _ = builder.Property(i => i.UnitPrice).HasColumnType("decimal(18,2)");
            _ = builder.Property(i => i.TaxAmount).HasColumnType("decimal(18,2)");
            _ = builder.Property(i => i.SubTotalPrice).HasColumnType("decimal(18,2)");
            _ = builder.Property(i => i.TotalPrice).HasColumnType("decimal(18,2)");
            _ = builder.Property(i => i.RowVersion).IsRowVersion();
            _ = builder.HasIndex(i => i.OrderId);
            _ = builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
        });

        modelBuilder.ConfigureOutboxEmail(ownsMigration: true);

        base.OnModelCreating(modelBuilder);
    }
}
