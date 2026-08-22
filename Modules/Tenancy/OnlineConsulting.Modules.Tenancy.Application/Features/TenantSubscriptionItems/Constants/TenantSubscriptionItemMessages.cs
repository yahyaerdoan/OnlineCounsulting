namespace OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptionItems.Constants;

public static class TenantSubscriptionItemMessages
{
    public const string TenantNotFound = "Tenant was not found.";
    public const string ModuleNotFound = "Module was not found or is not available for purchase.";
    public const string NoActiveSubscription = "This tenant has no active subscription to add or remove modules on.";
    public const string ModuleAlreadyAdded = "This module is already active on the tenant's subscription.";
    public const string ModuleNotActive = "This module is not currently active on the tenant's subscription.";
    public const string NotAuthorizedForTenant = "You are not authorized to manage this tenant's modules.";
    public const string ModuleBillingFailed = "We couldn't bill this module. Please try again in a few minutes.";
    public const string ModuleRemovalFailed = "We couldn't remove this module with the payment provider. Please try again in a few minutes.";
    public const string ModuleFeatureFlagFailed = "The module was billed and recorded but could not be enabled. Please contact support.";
    public const string MultipleModulesNotSupportedByProvider = "Your active payment provider doesn't support multiple modules in one subscription - please contact support.";
}
