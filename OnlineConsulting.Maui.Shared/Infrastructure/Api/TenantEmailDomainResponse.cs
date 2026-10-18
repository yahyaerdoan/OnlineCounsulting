using OnlineConsulting.Maui.Shared.Infrastructure.Hateoas;

namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors GET /api/v1/tenancy/my-tenant/email-domain; Status is "None", "Pending" or "Active".</summary>
public record TenantEmailDomainResponse(string? Domain, string Status, string? SenderAddress, List<TenantDomainDnsRecord> Records, string? Problem) : HalResource
{
    public bool IsActive => Status == "Active";
}
