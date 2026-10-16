using OnlineConsulting.Modules.Tenancy.Application.Features.TenantDomains.Contracts;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.TenantDomains.Abstractions;

/// <summary>Provider-side state of a domain; ready once DNS is verified and the certificate is issued.</summary>
public record CustomDomainSetup(string ProviderDomainId, bool IsReady, List<CustomDomainDnsRecord> Records, string? Problem);
