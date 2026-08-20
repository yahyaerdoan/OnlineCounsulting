using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.Referrals.Domain;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Referrals.Infrastructure.Persistence;

public class ReferralsDbContext(DbContextOptions<ReferralsDbContext> options, ITenantProvider tenantProvider) : DbContext(options)
{
    public DbSet<ReferralCode> ReferralCodes => Set<ReferralCode>();
    public DbSet<Referral> Referrals => Set<Referral>();
    public DbSet<AccountCredit> AccountCredits => Set<AccountCredit>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _ = modelBuilder.HasDefaultSchema("Referrals");

        _ = modelBuilder.Entity<ReferralCode>(builder =>
        {
            _ = builder.Property(c => c.Code).HasMaxLength(20).IsRequired();
            _ = builder.Property(c => c.RowVersion).IsRowVersion();
            _ = builder.HasIndex(c => c.UserId).IsUnique();
            _ = builder.HasIndex(c => c.Code).IsUnique();
            _ = builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
        });

        _ = modelBuilder.Entity<Referral>(builder =>
        {
            _ = builder.Property(r => r.Code).HasMaxLength(20).IsRequired();
            _ = builder.Property(r => r.Status).HasMaxLength(20).IsRequired();
            _ = builder.Property(r => r.RewardAmount).HasColumnType("decimal(18,2)");
            _ = builder.Property(r => r.RowVersion).IsRowVersion();
            _ = builder.HasIndex(r => r.ReferrerUserId);
            _ = builder.HasIndex(r => r.ReferredUserId).IsUnique();
            _ = builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
        });

        _ = modelBuilder.Entity<AccountCredit>(builder =>
        {
            _ = builder.Property(c => c.Amount).HasColumnType("decimal(18,2)");
            _ = builder.Property(c => c.Reason).HasMaxLength(200).IsRequired();
            _ = builder.Property(c => c.SourceType).HasMaxLength(50).IsRequired();
            _ = builder.Property(c => c.RowVersion).IsRowVersion();
            _ = builder.HasIndex(c => c.UserId);
            _ = builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
        });

        base.OnModelCreating(modelBuilder);
    }
}
