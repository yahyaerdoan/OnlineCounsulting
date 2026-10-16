namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>A DNS record the tenant adds; Name is relative to their domain.</summary>
public record TenantDomainDnsRecord(string Type, string Name, string Value, bool IsConfigured);
