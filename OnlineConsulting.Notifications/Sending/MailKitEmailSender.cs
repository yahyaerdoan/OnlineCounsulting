using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace OnlineConsulting.Notifications.Sending;

/// <summary>Port 465 uses TLS from the first byte (SslOnConnect); other ports upgrade with STARTTLS. Some networks block or stall
/// plaintext SMTP on 587, so 465 is the fallback that still works there. A short timeout keeps a stalled server from holding up the outbox.</summary>
public class MailKitEmailSender(IOptions<EmailOptions> options) : IEmailSender
{
    private const int ImplicitTlsPort = 465;
    private static readonly TimeSpan SmtpTimeout = TimeSpan.FromSeconds(30);

    private static SecureSocketOptions SecurityFor(EmailOptions settings) => settings switch
    {
        { UseSsl: false } => SecureSocketOptions.None,
        { SmtpPort: ImplicitTlsPort } => SecureSocketOptions.SslOnConnect,
        _ => SecureSocketOptions.StartTls,
    };

    public async Task SendAsync(string to, string subject, string htmlBody, string? cc, string? fromName, CancellationToken cancellationToken)
    {
        var settings = options.Value;

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(string.IsNullOrWhiteSpace(fromName) ? settings.FromName : fromName, settings.FromAddress));
        message.To.Add(MailboxAddress.Parse(to));
        if (!string.IsNullOrWhiteSpace(cc))
        {
            message.Cc.Add(MailboxAddress.Parse(cc));
        }

        message.Subject = subject;
        message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

        using var client = new SmtpClient { Timeout = (int)SmtpTimeout.TotalMilliseconds };
        await client.ConnectAsync(settings.SmtpHost, settings.SmtpPort, SecurityFor(settings), cancellationToken);
        await client.AuthenticateAsync(settings.Username, settings.Password, cancellationToken);
        _ = await client.SendAsync(message, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);
    }
}
