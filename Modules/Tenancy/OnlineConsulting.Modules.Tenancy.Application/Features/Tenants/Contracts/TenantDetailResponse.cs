using Hateoas;
using OnlineConsulting.Modules.Tenancy.Domain;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Contracts;

public record TenantDetailResponse(
    Guid Id,
    string Name,
    string Slug,
    string Status,
    string PrimaryContactEmail,
    Guid? OwnerUserId,
    string? SubscriptionStatus,
    DateTime? SubscriptionStartDate,
    DateTime? SubscriptionRenewalDate,
    List<TenantSubscriptionItemSummary> Items) : LinkedRecord
{
    /// <summary>Lists the subscription's modules not removed yet; load it with its items.</summary>
    public static TenantDetailResponse FromDomain(Tenant tenant, TenantSubscription? subscription) => new(
        tenant.Id, tenant.Name, tenant.Slug, tenant.Status, tenant.PrimaryContactEmail, tenant.OwnerUserId,
        subscription?.Status, subscription?.StartDate, subscription?.RenewalDate,
        [.. (subscription?.Items ?? []).Select(TenantSubscriptionItemSummary.FromDomain)]);
}
