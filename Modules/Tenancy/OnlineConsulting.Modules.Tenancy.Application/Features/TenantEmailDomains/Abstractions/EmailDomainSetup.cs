using OnlineConsulting.Modules.Tenancy.Application.Features.TenantDomains.Contracts;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.TenantEmailDomains.Abstractions;

/// <summary>Provider-side state of an email domain; ready once its DNS records authenticate it.</summary>
public record EmailDomainSetup(bool IsAuthenticated, List<CustomDomainDnsRecord> Records, string? Problem);
