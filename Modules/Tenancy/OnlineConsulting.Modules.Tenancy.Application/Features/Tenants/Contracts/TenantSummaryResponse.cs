using OnlineConsulting.Modules.Tenancy.Domain;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Contracts;

/// <summary>TotalActivePrice is next period's price: ending modules are left out. RenewalDate ends the current paid period.</summary>
public record TenantSummaryResponse(
    Guid Id,
    string Name,
    string Slug,
    string Status,
    string PrimaryContactEmail,
    List<string> ActiveModuleKeys,
    decimal TotalActivePrice,
    DateTimeOffset CreatedDate,
    string TimeZoneId,
    Guid? LogoMediaAssetId,
    bool IsStripeConnected,
    bool StripeChargesEnabled,
    List<EndingModuleResponse> EndingModules,
    DateTime? RenewalDate)
{
    public static TenantSummaryResponse FromDomain(Tenant tenant, IReadOnlyList<TenantSubscriptionItem> activeItems, DateTime? renewalDate) =>
        new(tenant.Id, tenant.Name, tenant.Slug, tenant.Status, tenant.PrimaryContactEmail, [.. activeItems.Select(i => i.ModuleKey)],
            activeItems.Where(i => !i.IsEnding).Sum(i => i.PriceAtAddition), tenant.CreatedDate, tenant.TimeZoneId, tenant.LogoMediaAssetId,
            tenant.StripeConnectedAccountId is not null, tenant.StripeChargesEnabled,
            [.. activeItems.Where(i => i.EndsAt is not null).Select(i => new EndingModuleResponse(i.ModuleKey, i.EndsAt ?? default))], renewalDate);
}
