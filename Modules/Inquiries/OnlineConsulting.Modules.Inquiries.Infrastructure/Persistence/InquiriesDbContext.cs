using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.Inquiries.Domain;
using OnlineConsulting.SharedKernel.Notifications;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Inquiries.Infrastructure.Persistence;

public class InquiriesDbContext(DbContextOptions<InquiriesDbContext> options, ITenantProvider tenantProvider) : DbContext(options)
{
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<NewsletterSubscriber> NewsletterSubscribers => Set<NewsletterSubscriber>();
    public DbSet<CompanyContact> CompanyContacts => Set<CompanyContact>();
    public DbSet<OutboxEmail> OutboxEmails => Set<OutboxEmail>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _ = modelBuilder.HasDefaultSchema("Inquiries");

        _ = modelBuilder.Entity<Message>(builder =>
        {
            _ = builder.Property(m => m.FirstName).HasMaxLength(100).IsRequired();
            _ = builder.Property(m => m.LastName).HasMaxLength(100).IsRequired();
            _ = builder.Property(m => m.Email).HasMaxLength(320).IsRequired();
            _ = builder.Property(m => m.Subject).HasMaxLength(200).IsRequired();
            _ = builder.Property(m => m.Description).HasMaxLength(4000).IsRequired();
            _ = builder.Property(m => m.RowVersion).IsRowVersion();
            _ = builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
        });

        _ = modelBuilder.Entity<NewsletterSubscriber>(builder =>
        {
            _ = builder.Property(s => s.Email).HasMaxLength(320).IsRequired();
            _ = builder.Property(s => s.RowVersion).IsRowVersion();
            _ = builder.HasIndex(s => s.Email);
            _ = builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
        });

        _ = modelBuilder.Entity<CompanyContact>(builder =>
        {
            _ = builder.Property(c => c.Email).HasMaxLength(320).IsRequired();
            _ = builder.Property(c => c.Phone).HasMaxLength(50).IsRequired();
            _ = builder.Property(c => c.Address).HasMaxLength(500).IsRequired();
            _ = builder.Property(c => c.Description).HasMaxLength(2000).IsRequired();
            _ = builder.Property(c => c.WorkingHours).HasMaxLength(200).IsRequired();
            _ = builder.Property(c => c.RowVersion).IsRowVersion();
            _ = builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
        });

        modelBuilder.ConfigureOutboxEmail(ownsMigration: false);

        base.OnModelCreating(modelBuilder);
    }
}
