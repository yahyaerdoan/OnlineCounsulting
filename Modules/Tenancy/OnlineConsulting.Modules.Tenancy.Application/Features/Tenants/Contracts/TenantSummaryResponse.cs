using OnlineConsulting.Modules.Tenancy.Domain;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Contracts;

public record TenantSummaryResponse(
    Guid Id,
    string Name,
    string Slug,
    string Status,
    string PrimaryContactEmail,
    List<string> ActiveModuleKeys,
    decimal TotalActivePrice)
{
    public static TenantSummaryResponse FromDomain(Tenant tenant, List<string> activeModuleKeys, decimal totalActivePrice) => new(
        tenant.Id, tenant.Name, tenant.Slug, tenant.Status, tenant.PrimaryContactEmail, activeModuleKeys, totalActivePrice);
}
