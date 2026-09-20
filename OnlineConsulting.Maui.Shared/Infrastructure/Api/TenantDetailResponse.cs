namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors Tenancy's TenantSubscriptionItemSummary - one module's billing history line.</summary>
public record TenantSubscriptionItemSummary(
    string ModuleKey,
    string Status,
    decimal PriceAtAddition,
    DateTime AddedAt);

/// <summary>Mirrors Tenancy's TenantDetailResponse - a tenant plus its subscription lifecycle and every module ever billed on it.</summary>
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
    List<TenantSubscriptionItemSummary> Items);
