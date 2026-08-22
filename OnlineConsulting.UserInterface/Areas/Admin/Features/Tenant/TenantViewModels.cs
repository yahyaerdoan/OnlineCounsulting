namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Tenant;

public record TenantListItemViewModel(
    Guid Id,
    string Name,
    string Slug,
    string Status,
    string PrimaryContactEmail,
    List<string> ActiveModuleKeys,
    decimal TotalActivePrice);

public record TenantSubscriptionItemViewModel(string ModuleKey, string Status, decimal PriceAtAddition, DateTime AddedAt);

public record ModuleOfferingViewModel(string Key, string Name, decimal Price, string BillingCycle);

public record TenantDetailViewModel(
    Guid Id,
    string Name,
    string Slug,
    string Status,
    string PrimaryContactEmail,
    Guid? OwnerUserId,
    string? SubscriptionStatus,
    DateTime? SubscriptionStartDate,
    DateTime? SubscriptionRenewalDate,
    List<TenantSubscriptionItemViewModel> Items,
    List<ModuleOfferingViewModel> AvailableModules);
