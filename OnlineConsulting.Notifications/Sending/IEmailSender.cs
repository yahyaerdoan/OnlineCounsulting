namespace OnlineConsulting.Notifications.Sending;

/// <summary>Sends one email over SMTP; only the outbox dispatcher calls it, modules write to the outbox.</summary>
public interface IEmailSender
{
    /// <summary>Throws on failure so the dispatcher can retry; cc is optional.</summary>
    Task SendAsync(string to, string subject, string htmlBody, string? cc, CancellationToken cancellationToken);
}
