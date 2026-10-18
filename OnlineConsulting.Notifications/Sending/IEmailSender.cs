namespace OnlineConsulting.Notifications.Sending;

/// <summary>Sends one email; only the outbox dispatcher calls it, modules write to the outbox.</summary>
public interface IEmailSender
{
    /// <summary>Throws on failure so the dispatcher can retry; cc is optional, fromName and fromAddress are the sending business (configured values when empty).</summary>
    Task SendAsync(string to, string subject, string htmlBody, string? cc, string? fromName, string? fromAddress, CancellationToken cancellationToken);
}
