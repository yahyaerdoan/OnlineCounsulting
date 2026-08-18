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
        modelBuilder.HasDefaultSchema("Memberships");

        modelBuilder.Entity<MembershipPlan>(builder =>
        {
            builder.Property(p => p.Name).HasMaxLength(200).IsRequired();
            builder.Property(p => p.BillingCycle).HasMaxLength(30).IsRequired();
            builder.Property(p => p.Price).HasColumnType("decimal(18,2)");
            builder.Property(p => p.DiscountPercent).HasColumnType("decimal(5,2)");
            builder.Property(p => p.CreditAmount).HasColumnType("decimal(18,2)");
            builder.Property(p => p.Benefits).HasMaxLength(2000);
            builder.Property(p => p.RowVersion).IsRowVersion();
            builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
        });

        modelBuilder.Entity<CustomerMembership>(builder =>
        {
            builder.Property(m => m.Status).HasMaxLength(30).IsRequired();
            builder.Property(m => m.RowVersion).IsRowVersion();
            builder.HasIndex(m => m.UserId);
            builder.HasIndex(m => m.MembershipPlanId);
            builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
        });

        base.OnModelCreating(modelBuilder);
    }
}
