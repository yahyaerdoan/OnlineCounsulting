using OnlineConsulting.Maui.Shared.Infrastructure.Hateoas;

namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors Tenancy's TenantSummaryResponse - backs the paginated Tenants admin table.</summary>
public record TenantResponse(
    Guid Id,
    string Name,
    string Slug,
    string Status,
    string PrimaryContactEmail,
    List<string> ActiveModuleKeys,
    decimal TotalActivePrice,
    DateTimeOffset? CreatedDate = null,
    string? TimeZoneId = null) : HalResource, IQueryableFields
{
    public static string[] SearchFields => [nameof(Name), nameof(Slug), nameof(PrimaryContactEmail)];
}
