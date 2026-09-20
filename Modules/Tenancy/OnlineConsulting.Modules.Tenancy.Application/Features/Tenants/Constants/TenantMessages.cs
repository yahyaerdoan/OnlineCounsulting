namespace OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Constants;

public static class TenantMessages
{
    public const string TenantNotFound = "Tenant was not found.";
    public const string AlreadySuspended = "This tenant is already suspended.";
    public const string NotSuspendable = "Only Active or PastDue tenants can be suspended.";
    public const string NotReactivatable = "Only Suspended tenants can be reactivated.";
    public const string NotCancellable = "This tenant is already cancelled.";
    public const string CancellationFailed = "We couldn't cancel this tenant's subscription with the payment provider. Please try again in a few minutes.";
}
