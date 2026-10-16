using OnlineConsulting.Maui.Shared.Infrastructure.Hateoas;

namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors GET /api/v1/tenancy/my-tenant/domain; Status is "None", "Pending" or "Active".</summary>
public record TenantDomainResponse(string? Host, string Status, List<TenantDomainDnsRecord> Records, string? Problem) : HalResource
{
    public bool IsActive => Status == "Active";
}
