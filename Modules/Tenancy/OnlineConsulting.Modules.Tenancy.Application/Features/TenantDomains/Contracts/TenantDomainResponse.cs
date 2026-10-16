using OnlineConsulting.Modules.Tenancy.Application.Features.TenantDomains.Abstractions;
using OnlineConsulting.Modules.Tenancy.Domain;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.TenantDomains.Contracts;

/// <summary>The caller's custom domain; <see cref="Host"/> is null and <see cref="Status"/> is <see cref="NoneStatus"/> when none is set up.</summary>
public record TenantDomainResponse(string? Host, string Status, List<CustomDomainDnsRecord> Records, string? Problem)
{
    public const string NoneStatus = "None";

    public static TenantDomainResponse None() => new(null, NoneStatus, [], null);

    public static TenantDomainResponse FromDomain(TenantDomain domain, CustomDomainSetup? setup) =>
        new(domain.Host, domain.Status, domain.IsActive ? [] : setup?.Records ?? [], domain.IsActive ? null : setup?.Problem);
}
