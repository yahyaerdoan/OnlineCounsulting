using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantDomains.Contracts;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantEmailDomains.Abstractions;

namespace OnlineConsulting.Modules.Tenancy.Infrastructure.EmailDomains;

/// <summary>Registers tenant email domains as Brevo sender domains, so mail from them is DKIM-signed.</summary>
public class BrevoEmailDomainGateway(IHttpClientFactory httpClientFactory, IOptions<BrevoEmailDomainOptions> options) : IEmailDomainGateway
{
    private const string ApiKeyHeader = "api-key";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web) { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };

    private BrevoEmailDomainOptions Settings => options.Value;

    public async Task<EmailDomainSetup> AddAsync(string domain, CancellationToken cancellationToken = default)
    {
        // A domain already in the Brevo account (e.g. re-added after a remove) is fine; read it instead.
        using var response = await SendAsync(HttpMethod.Post, "senders/domains", new { name = domain }, cancellationToken);
        if (response.StatusCode != HttpStatusCode.BadRequest)
        {
            await EnsureSuccessAsync(response, cancellationToken);
        }

        return await GetAsync(domain, cancellationToken);
    }

    public async Task<EmailDomainSetup> GetAsync(string domain, CancellationToken cancellationToken = default)
    {
        using var response = await SendAsync(HttpMethod.Get, DomainPath(domain), null, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);

        var config = await response.Content.ReadFromJsonAsync<DomainConfig>(JsonOptions, cancellationToken)
            ?? throw new InvalidOperationException("Brevo API returned no domain.");

        return ToSetup(domain, config);
    }

    public async Task<EmailDomainSetup> AuthenticateAsync(string domain, string senderAddress, string senderName, CancellationToken cancellationToken = default)
    {
        // 400 means the records aren't in place yet; the GET below reports which.
        using (var response = await SendAsync(HttpMethod.Put, $"{DomainPath(domain)}/authenticate", null, cancellationToken))
        {
            if (response.StatusCode != HttpStatusCode.BadRequest)
            {
                await EnsureSuccessAsync(response, cancellationToken);
            }
        }

        var setup = await GetAsync(domain, cancellationToken);
        if (setup.IsAuthenticated)
        {
            // 400 here means the sender already exists.
            using var sender = await SendAsync(HttpMethod.Post, "senders", new { name = senderName, email = senderAddress }, cancellationToken);
            if (sender.StatusCode != HttpStatusCode.BadRequest)
            {
                await EnsureSuccessAsync(sender, cancellationToken);
            }
        }

        return setup;
    }

    public async Task RemoveAsync(string domain, CancellationToken cancellationToken = default)
    {
        using var response = await SendAsync(HttpMethod.Delete, DomainPath(domain), null, cancellationToken);
        if (response.StatusCode != HttpStatusCode.NotFound)
        {
            await EnsureSuccessAsync(response, cancellationToken);
        }
    }

    private static string DomainPath(string domain) => $"senders/domains/{Uri.EscapeDataString(domain)}";

    private async Task<HttpResponseMessage> SendAsync(HttpMethod method, string path, object? body, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(method, $"{Settings.ApiUrl.TrimEnd('/')}/{path}");
        request.Headers.Add(ApiKeyHeader, Settings.ApiKey);
        if (body is not null)
        {
            request.Content = JsonContent.Create(body);
        }

        var client = httpClientFactory.CreateClient(nameof(BrevoEmailDomainGateway));
        return await client.SendAsync(request, cancellationToken);
    }

    // Brevo's body names the cause (bad key, unknown domain).
    private static async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"Brevo API error ({(int)response.StatusCode}): {body}", null, response.StatusCode);
        }
    }

    private static EmailDomainSetup ToSetup(string domain, DomainConfig config)
    {
        // Keys vary by account (dkim_record, or dkim1/dkim2 CNAMEs), so every entry is taken.
        var records = (config.DnsRecords ?? [])
            .Values
            .OfType<DnsRecord>()
            .Select(r => new CustomDomainDnsRecord(r.Type, RelativeName(r.HostName, domain), r.Value, r.Status))
            .ToList();

        return new EmailDomainSetup(config.Authenticated, records, null);
    }

    /// <summary>"mail._domainkey" for "mail._domainkey.example.com", "@" for the domain itself.</summary>
    private static string RelativeName(string? hostName, string domain)
    {
        if (string.IsNullOrWhiteSpace(hostName) || hostName.Equals(domain, StringComparison.OrdinalIgnoreCase))
        {
            return "@";
        }

        var suffix = $".{domain}";
        return hostName.EndsWith(suffix, StringComparison.OrdinalIgnoreCase) ? hostName[..^suffix.Length] : hostName;
    }

    private sealed record DomainConfig(string Domain, bool Verified, bool Authenticated, Dictionary<string, DnsRecord?>? DnsRecords);

    private sealed record DnsRecord(string Type, string Value, string? HostName, bool Status);
}
