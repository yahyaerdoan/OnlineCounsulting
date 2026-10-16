using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantDomains.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantDomains.Contracts;

namespace OnlineConsulting.Modules.Tenancy.Infrastructure.Domains;

/// <summary>Adds tenant domains to the Railway service that serves tenant sites, through Railway's GraphQL API.</summary>
public class RailwayCustomDomainGateway(IHttpClientFactory httpClientFactory, IOptions<TenantDomainOptions> options) : ICustomDomainGateway
{
    private const string TokenHeader = "Project-Access-Token";
    private const string DomainFields = "id status { verified verificationDnsHost verificationToken certificateStatus certificateErrorMessage dnsRecords { hostlabel recordType requiredValue status zone } }";
    private const string CertificateValid = "CERTIFICATE_STATUS_TYPE_VALID";
    private const string RecordPropagated = "DNS_RECORD_STATUS_PROPAGATED";
    private const string RecordTypePrefix = "DNS_RECORD_TYPE_";

    private RailwayDomainOptions Settings => options.Value.Railway;

    public async Task<CustomDomainSetup> AddAsync(string host, CancellationToken cancellationToken = default)
    {
        var input = new { domain = host, projectId = Settings.ProjectId, environmentId = Settings.EnvironmentId, serviceId = Settings.ServiceId, targetPort = Settings.TargetPort };
        var data = await SendAsync<CreateData>($"mutation($input: CustomDomainCreateInput!) {{ customDomainCreate(input: $input) {{ {DomainFields} }} }}", new { input }, cancellationToken);
        return ToSetup(data.CustomDomainCreate);
    }

    public async Task<CustomDomainSetup> GetAsync(string providerDomainId, CancellationToken cancellationToken = default)
    {
        var data = await SendAsync<GetData>($"query($id: String!, $projectId: String!) {{ customDomain(id: $id, projectId: $projectId) {{ {DomainFields} }} }}",
            new { id = providerDomainId, projectId = Settings.ProjectId }, cancellationToken);
        return ToSetup(data.CustomDomain);
    }

    public async Task RemoveAsync(string providerDomainId, CancellationToken cancellationToken = default) =>
        _ = await SendAsync<DeleteData>("mutation($id: String!) { customDomainDelete(id: $id) }", new { id = providerDomainId }, cancellationToken);

    private async Task<TData> SendAsync<TData>(string query, object variables, CancellationToken cancellationToken)
        where TData : class
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, Settings.ApiUrl) { Content = JsonContent.Create(new { query, variables }) };
        request.Headers.Add(TokenHeader, Settings.ProjectToken);

        using var client = httpClientFactory.CreateClient(nameof(RailwayCustomDomainGateway));
        using var response = await client.SendAsync(request, cancellationToken);
        _ = response.EnsureSuccessStatusCode();

        // GraphQL reports failures, including auth, as 200 with errors.
        var body = await response.Content.ReadFromJsonAsync<GraphQlResponse<TData>>(cancellationToken: cancellationToken);
        if (body?.Errors is { Count: > 0 } errors)
        {
            throw new InvalidOperationException($"Railway API error: {string.Join("; ", errors.Select(e => e.Message))}");
        }

        return body?.Data ?? throw new InvalidOperationException("Railway API returned no data.");
    }

    private static CustomDomainSetup ToSetup(RailwayDomain domain)
    {
        var status = domain.Status;
        var records = status.DnsRecords
            .Select(r => new CustomDomainDnsRecord(r.RecordType.Replace(RecordTypePrefix, string.Empty), NameOrApex(r.Hostlabel), r.RequiredValue, r.Status == RecordPropagated))
            .ToList();

        // Ownership TXT record, needed until Railway verifies the domain.
        if (!status.Verified && status is { VerificationDnsHost: { Length: > 0 } verificationHost, VerificationToken: { Length: > 0 } token })
        {
            var zone = status.DnsRecords.FirstOrDefault()?.Zone;
            var name = zone is { Length: > 0 } && verificationHost.EndsWith($".{zone}", StringComparison.OrdinalIgnoreCase) ? verificationHost[..^(zone.Length + 1)] : verificationHost;
            records.Add(new CustomDomainDnsRecord("TXT", name, token, false));
        }

        var isReady = status.Verified && status.CertificateStatus == CertificateValid;
        return new CustomDomainSetup(domain.Id, isReady, records, status.CertificateErrorMessage);
    }

    private static string NameOrApex(string hostLabel) => string.IsNullOrEmpty(hostLabel) ? "@" : hostLabel;

    private sealed record GraphQlResponse<TData>(TData? Data, List<GraphQlError>? Errors)
        where TData : class;

    private sealed record GraphQlError(string Message);

    private sealed record CreateData(RailwayDomain CustomDomainCreate);

    private sealed record GetData(RailwayDomain CustomDomain);

    private sealed record DeleteData(bool CustomDomainDelete);

    private sealed record RailwayDomain(string Id, RailwayDomainStatus Status);

    private sealed record RailwayDomainStatus(bool Verified, string? VerificationDnsHost, string? VerificationToken, string CertificateStatus, string? CertificateErrorMessage,
        List<RailwayDnsRecord> DnsRecords);

    private sealed record RailwayDnsRecord(string Hostlabel, string RecordType, string RequiredValue, string Status, string? Zone);
}
