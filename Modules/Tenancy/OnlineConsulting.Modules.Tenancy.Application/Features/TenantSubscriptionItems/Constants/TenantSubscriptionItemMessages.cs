namespace OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptionItems.Constants;

public static class TenantSubscriptionItemMessages
{
    public const string TenantNotFound = "Tenant was not found.";
    public const string ModuleNotFound = "Module was not found or is not available for purchase.";
    public const string NoActiveSubscription = "This tenant has no active subscription to add or remove modules on.";
    public const string ModuleAlreadyAdded = "This module is already active on the tenant's subscription.";
    public const string ModuleNotActive = "This module is not currently active on the tenant's subscription.";
    public const string NotAuthorizedForTenant = "You are not authorized to manage this tenant's modules.";
}
