using Microsoft.EntityFrameworkCore;

namespace OnlineConsulting.SharedKernel.Notifications;

public static class OutboxEmailModelBuilderExtensions
{
    /// <summary>Maps OutboxEmail to the same fixed schema/table for every DbContext that calls this.</summary>
    public static void ConfigureOutboxEmail(this ModelBuilder modelBuilder)
    {
        _ = modelBuilder.Entity<OutboxEmail>(builder =>
        {
            _ = builder.ToTable("OutboxEmails", "Notifications");
            _ = builder.Property(e => e.To).HasMaxLength(320).IsRequired();
            _ = builder.Property(e => e.Cc).HasMaxLength(320);
            _ = builder.Property(e => e.Subject).HasMaxLength(500).IsRequired();
            _ = builder.Property(e => e.HtmlBody).IsRequired();
            _ = builder.Property(e => e.SourceReference).HasMaxLength(200);
            _ = builder.Property(e => e.LastError).HasMaxLength(2000);
            _ = builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(20);
            _ = builder.Property(e => e.RowVersion).IsRowVersion();
            _ = builder.HasIndex(e => new { e.Status, e.NextAttemptAt });
        });
    }
}
