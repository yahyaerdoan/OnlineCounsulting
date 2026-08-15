using Microsoft.EntityFrameworkCore;

namespace OnlineConsulting.SharedKernel.Notifications;

public static class OutboxEmailModelBuilderExtensions
{
    /// <summary>Maps OutboxEmail to the same fixed schema/table for every DbContext that calls this.</summary>
    public static void ConfigureOutboxEmail(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OutboxEmail>(builder =>
        {
            builder.ToTable("OutboxEmails", "Notifications");
            builder.Property(e => e.To).HasMaxLength(320).IsRequired();
            builder.Property(e => e.Cc).HasMaxLength(320);
            builder.Property(e => e.Subject).HasMaxLength(500).IsRequired();
            builder.Property(e => e.HtmlBody).IsRequired();
            builder.Property(e => e.SourceReference).HasMaxLength(200);
            builder.Property(e => e.LastError).HasMaxLength(2000);
            builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(20);
            builder.Property(e => e.RowVersion).IsRowVersion();
            builder.HasIndex(e => new { e.Status, e.NextAttemptAt });
        });
    }
}
