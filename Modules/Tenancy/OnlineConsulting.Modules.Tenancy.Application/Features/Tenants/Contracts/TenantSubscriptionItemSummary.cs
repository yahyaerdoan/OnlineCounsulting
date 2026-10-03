using OnlineConsulting.Modules.Tenancy.Domain;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Contracts;

public record TenantSubscriptionItemSummary(
    string ModuleKey,
    string Status,
    decimal PriceAtAddition,
    DateTime AddedAt)
{
    public static TenantSubscriptionItemSummary FromDomain(TenantSubscriptionItem item) => new(
        item.ModuleKey, item.Status, item.PriceAtAddition, item.AddedAt);
}
