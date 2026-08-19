namespace OnlineConsulting.Modules.Tenancy.Application.Features.Signup.Constants;

public static class SignupMessages
{
    public const string SlugAlreadyTaken = "A company with a similar name is already registered. Please choose a different company name.";
    public const string NoModulesSelected = "At least one module must be selected.";
    public const string UnknownOrUnavailableModuleKeysFormat = "The following modules were not found or are not available for purchase: {0}.";
    public const string PaymentSetupFailed = "We couldn't complete payment setup for your subscription. Please try again in a few minutes, or contact support if the problem persists.";
    public const string TenantNotFound = "Tenant was not found.";
    public const string MultipleModulesNotSupportedByProvider = "Your active payment provider doesn't support multiple modules in one subscription - please select a single module, or contact support.";
}
