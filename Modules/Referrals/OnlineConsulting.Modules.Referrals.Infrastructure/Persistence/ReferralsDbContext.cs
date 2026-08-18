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
        modelBuilder.HasDefaultSchema("Referrals");

        modelBuilder.Entity<ReferralCode>(builder =>
        {
            builder.Property(c => c.Code).HasMaxLength(20).IsRequired();
            builder.Property(c => c.RowVersion).IsRowVersion();
            builder.HasIndex(c => c.UserId).IsUnique();
            builder.HasIndex(c => c.Code).IsUnique();
            builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
        });

        modelBuilder.Entity<Referral>(builder =>
        {
            builder.Property(r => r.Code).HasMaxLength(20).IsRequired();
            builder.Property(r => r.Status).HasMaxLength(20).IsRequired();
            builder.Property(r => r.RewardAmount).HasColumnType("decimal(18,2)");
            builder.Property(r => r.RowVersion).IsRowVersion();
            builder.HasIndex(r => r.ReferrerUserId);
            builder.HasIndex(r => r.ReferredUserId).IsUnique();
            builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
        });

        modelBuilder.Entity<AccountCredit>(builder =>
        {
            builder.Property(c => c.Amount).HasColumnType("decimal(18,2)");
            builder.Property(c => c.Reason).HasMaxLength(200).IsRequired();
            builder.Property(c => c.SourceType).HasMaxLength(50).IsRequired();
            builder.Property(c => c.RowVersion).IsRowVersion();
            builder.HasIndex(c => c.UserId);
            builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
        });

        base.OnModelCreating(modelBuilder);
    }
}
