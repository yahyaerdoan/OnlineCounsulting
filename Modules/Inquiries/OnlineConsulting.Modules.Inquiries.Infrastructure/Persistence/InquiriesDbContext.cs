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
        modelBuilder.HasDefaultSchema("Inquiries");

        modelBuilder.Entity<Message>(builder =>
        {
            builder.Property(m => m.FirstName).HasMaxLength(100).IsRequired();
            builder.Property(m => m.LastName).HasMaxLength(100).IsRequired();
            builder.Property(m => m.Email).HasMaxLength(320).IsRequired();
            builder.Property(m => m.Subject).HasMaxLength(200).IsRequired();
            builder.Property(m => m.Description).HasMaxLength(4000).IsRequired();
            builder.Property(m => m.RowVersion).IsRowVersion();
            builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
        });

        modelBuilder.Entity<NewsletterSubscriber>(builder =>
        {
            builder.Property(s => s.Email).HasMaxLength(320).IsRequired();
            builder.Property(s => s.RowVersion).IsRowVersion();
            builder.HasIndex(s => s.Email);
            builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
        });

        modelBuilder.Entity<CompanyContact>(builder =>
        {
            builder.Property(c => c.Email).HasMaxLength(320).IsRequired();
            builder.Property(c => c.Phone).HasMaxLength(50).IsRequired();
            builder.Property(c => c.Address).HasMaxLength(500).IsRequired();
            builder.Property(c => c.Description).HasMaxLength(2000).IsRequired();
            builder.Property(c => c.WorkingHours).HasMaxLength(200).IsRequired();
            builder.Property(c => c.RowVersion).IsRowVersion();
            builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
        });

        modelBuilder.ConfigureOutboxEmail();

        base.OnModelCreating(modelBuilder);
    }
}
