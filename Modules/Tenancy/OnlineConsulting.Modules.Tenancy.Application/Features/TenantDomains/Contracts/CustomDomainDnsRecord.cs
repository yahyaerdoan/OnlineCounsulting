namespace OnlineConsulting.Modules.Tenancy.Application.Features.TenantDomains.Contracts;

/// <summary>A record the tenant adds at their DNS provider; <see cref="Name"/> is relative to their domain.</summary>
public record CustomDomainDnsRecord(string Type, string Name, string Value, bool IsConfigured);
