namespace OnlineConsulting.Notifications.Sending;

public interface IEmailSender
{
    Task SendAsync(string to, string subject, string htmlBody, string? cc, CancellationToken cancellationToken);
}
