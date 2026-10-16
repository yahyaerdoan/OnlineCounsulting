using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantDomains.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantDomains.Contracts;

namespace OnlineConsulting.Modules.Tenancy.Infrastructure.Domains;

/// <summary>Registers tenant domains as Cloudflare for SaaS custom hostnames. Customers point one CNAME at our zone,
/// so moving servers only changes the edge Worker's origin, never their DNS.</summary>
public class CloudflareCustomDomainGateway(IHttpClientFactory httpClientFactory, IOptions<TenantDomainOptions> options) : ICustomDomainGateway
{
    private const string Active = "active";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web) { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };

    // Final states that need a person; anything else is still in progress.
    private static readonly string[] FailedStatuses = ["blocked", "moved", "deleted", "validation_timed_out", "issuance_timed_out", "deployment_timed_out", "deletion_timed_out"];

    private CloudflareDomainOptions Settings => options.Value.Cloudflare;

    private string HostnamesPath => $"{Settings.ApiUrl.TrimEnd('/')}/zones/{Settings.ZoneId}/custom_hostnames";

    public async Task<CustomDomainSetup> AddAsync(string host, CancellationToken cancellationToken = default)
    {
        // HTTP validation: the certificate is issued once the customer's CNAME reaches us, no extra record.
        var body = new { hostname = host, ssl = new { method = "http", type = "dv" } };
        var hostname = await SendAsync<CustomHostname>(HttpMethod.Post, HostnamesPath, body, cancellationToken);
        return ToSetup(hostname);
    }

    public async Task<CustomDomainSetup> GetAsync(string providerDomainId, CancellationToken cancellationToken = default)
    {
        var hostname = await SendAsync<CustomHostname>(HttpMethod.Get, $"{HostnamesPath}/{providerDomainId}", null, cancellationToken);
        return ToSetup(hostname);
    }

    public async Task RemoveAsync(string providerDomainId, CancellationToken cancellationToken = default) =>
        _ = await SendAsync<DeletedHostname>(HttpMethod.Delete, $"{HostnamesPath}/{providerDomainId}", null, cancellationToken);

    private async Task<TResult> SendAsync<TResult>(HttpMethod method, string url, object? body, CancellationToken cancellationToken)
        where TResult : class
    {
        using var request = new HttpRequestMessage(method, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Settings.ApiToken);
        if (body is not null)
        {
            request.Content = JsonContent.Create(body, options: JsonOptions);
        }

        using var client = httpClientFactory.CreateClient(nameof(CloudflareCustomDomainGateway));
        using var response = await client.SendAsync(request, cancellationToken);

        // Failures carry their reason in the envelope, so read it before checking the status code.
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<TResult>>(JsonOptions, cancellationToken);
        if (envelope is not { Success: true, Result: { } result })
        {
            var reason = envelope?.Errors is { Count: > 0 } errors ? string.Join("; ", errors.Select(e => e.Message)) : response.StatusCode.ToString();
            throw new InvalidOperationException($"Cloudflare API error: {reason}");
        }

        return result;
    }

    private CustomDomainSetup ToSetup(CustomHostname hostname)
    {
        var sslStatus = hostname.Ssl?.Status ?? string.Empty;
        var isRouted = hostname.Status == Active;
        var records = new List<CustomDomainDnsRecord> { new("CNAME", DnsRecordNames.Relative(hostname.Hostname), Settings.CnameTarget, isRouted) };

        string? problem = null;
        if (FailedStatuses.Contains(hostname.Status) || FailedStatuses.Contains(sslStatus))
        {
            problem = hostname.VerificationErrors?.FirstOrDefault() ?? hostname.Ssl?.ValidationErrors?.FirstOrDefault()?.Message ?? $"{hostname.Status} / {sslStatus}";
        }

        return new CustomDomainSetup(hostname.Id, isRouted && sslStatus == Active, records, problem);
    }

    private sealed record Envelope<TResult>(bool Success, List<ApiError>? Errors, TResult? Result)
        where TResult : class;

    private sealed record ApiError(string Message);

    private sealed record CustomHostname(string Id, string Hostname, string Status, HostnameSsl? Ssl, List<string>? VerificationErrors);

    private sealed record HostnameSsl(string? Status, List<ApiError>? ValidationErrors);

    private sealed record DeletedHostname(string Id);
}
