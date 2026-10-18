using OnlineConsulting.Modules.Tenancy.Application.Features.TenantDomains.Contracts;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantEmailDomains.Abstractions;
using OnlineConsulting.Modules.Tenancy.Domain;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.TenantEmailDomains.Contracts;

/// <summary>The caller's email domain; <see cref="Domain"/> is null and <see cref="Status"/> is <see cref="NoneStatus"/> when none is set up.</summary>
public record TenantEmailDomainResponse(string? Domain, string Status, string? SenderAddress, List<CustomDomainDnsRecord> Records, string? Problem)
{
    public const string NoneStatus = "None";

    public static TenantEmailDomainResponse None() => new(null, NoneStatus, null, [], null);

    public static TenantEmailDomainResponse FromDomain(TenantEmailDomain domain, EmailDomainSetup? setup) =>
        new(domain.Domain, domain.Status, domain.SenderAddress, domain.IsActive ? [] : setup?.Records ?? [], domain.IsActive ? null : setup?.Problem);
}
