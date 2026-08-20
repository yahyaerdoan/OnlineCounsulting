using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.Memberships.Domain;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Memberships.Infrastructure.Persistence;

public class MembershipsDbContext(DbContextOptions<MembershipsDbContext> options, ITenantProvider tenantProvider) : DbContext(options)
{
    public DbSet<MembershipPlan> MembershipPlans => Set<MembershipPlan>();
    public DbSet<CustomerMembership> CustomerMemberships => Set<CustomerMembership>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _ = modelBuilder.HasDefaultSchema("Memberships");

        _ = modelBuilder.Entity<MembershipPlan>(builder =>
        {
            _ = builder.Property(p => p.Name).HasMaxLength(200).IsRequired();
            _ = builder.Property(p => p.BillingCycle).HasMaxLength(30).IsRequired();
            _ = builder.Property(p => p.Price).HasColumnType("decimal(18,2)");
            _ = builder.Property(p => p.DiscountPercent).HasColumnType("decimal(5,2)");
            _ = builder.Property(p => p.CreditAmount).HasColumnType("decimal(18,2)");
            _ = builder.Property(p => p.Benefits).HasMaxLength(2000);
            _ = builder.Property(p => p.RowVersion).IsRowVersion();
            _ = builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
        });

        _ = modelBuilder.Entity<CustomerMembership>(builder =>
        {
            _ = builder.Property(m => m.Status).HasMaxLength(30).IsRequired();
            _ = builder.Property(m => m.RowVersion).IsRowVersion();
            _ = builder.HasIndex(m => m.UserId);
            _ = builder.HasIndex(m => m.MembershipPlanId);
            _ = builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
        });

        base.OnModelCreating(modelBuilder);
    }
}
