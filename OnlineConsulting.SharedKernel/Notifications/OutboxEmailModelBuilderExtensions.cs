using Microsoft.EntityFrameworkCore;

namespace OnlineConsulting.SharedKernel.Notifications;

public static class OutboxEmailModelBuilderExtensions
{
    /// <summary>Maps OutboxEmail to the shared OutboxEmails table. Only one caller should pass
    /// <paramref name="ownsMigration"/>: true - others must pass false or migrations duplicate the table.</summary>
    public static void ConfigureOutboxEmail(this ModelBuilder modelBuilder, bool ownsMigration)
    {
        _ = modelBuilder.Entity<OutboxEmail>(builder =>
        {
            _ = builder.ToTable("OutboxEmails", "Notifications", t =>
            {
                if (!ownsMigration)
                {
                    _ = t.ExcludeFromMigrations();
                }
            });
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
