using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Notifications.Sending;

/// <summary>Sends over Brevo's HTTPS API - for hosts that block outbound SMTP (e.g. Railway below Pro).</summary>
public class BrevoEmailSender(IHttpClientFactory httpClientFactory, IOptions<EmailOptions> options) : IEmailSender
{
    public const string BaseAddress = "https://api.brevo.com";
    private const string SendPath = "/v3/smtp/email";
    private const string ApiKeyHeader = "api-key";

    // Brevo rejects "cc": null, so nulls are omitted.
    private static readonly JsonSerializerOptions JsonOptions = new() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };

    public async Task SendAsync(string to, string subject, string htmlBody, string? cc, string? fromName, CancellationToken cancellationToken)
    {
        var settings = options.Value;

        using var request = new HttpRequestMessage(HttpMethod.Post, SendPath);
        request.Headers.Add(ApiKeyHeader, settings.Brevo.ApiKey);
        request.Content = JsonContent.Create(new
        {
            sender = new { name = string.IsNullOrWhiteSpace(fromName) ? settings.FromName : fromName, email = settings.FromAddress },
            to = new[] { new { email = to } },
            cc = string.IsNullOrWhiteSpace(cc) ? null : new[] { new { email = cc } },
            subject,
            htmlContent = htmlBody,
        }, options: JsonOptions);

        using var client = httpClientFactory.CreateClient(nameof(BrevoEmailSender));
        using var response = await client.SendAsync(request, cancellationToken);

        // Brevo's body names the cause (unverified sender, bad key) - keep it for the outbox's LastError.
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"Brevo send failed ({(int)response.StatusCode}): {body}", null, response.StatusCode);
        }
    }
}
