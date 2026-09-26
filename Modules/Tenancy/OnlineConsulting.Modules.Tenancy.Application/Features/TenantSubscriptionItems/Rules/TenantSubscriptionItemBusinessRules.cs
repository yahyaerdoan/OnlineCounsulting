using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptionItems.Constants;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptionItems.Rules;

public static class TenantSubscriptionItemBusinessRules
{
    public static OperationResult NotAuthorizedForTenant() =>
        Result.Forbidden(TenantSubscriptionItemMessages.NotAuthorizedForTenant);

    public static OperationResult NoActiveSubscription() =>
        Result.Conflict(TenantSubscriptionItemMessages.NoActiveSubscription);

    public static OperationResult TenantNotFound() =>
        Result.NotFound(TenantSubscriptionItemMessages.TenantNotFound);

    public static OperationResult ModuleNotFound() =>
        Result.NotFound(TenantSubscriptionItemMessages.ModuleNotFound);

    public static OperationResult ModuleAlreadyAdded() =>
        Result.Conflict(TenantSubscriptionItemMessages.ModuleAlreadyAdded);

    public static OperationResult ModuleNotActive() =>
        Result.NotFound(TenantSubscriptionItemMessages.ModuleNotActive);

    public static OperationResult CannotRemoveLastModule() =>
        Result.Conflict(TenantSubscriptionItemMessages.CannotRemoveLastModule);

    public static OperationResult MultipleModulesNotSupportedByProvider() =>
        Result.UnprocessableContent(TenantSubscriptionItemMessages.MultipleModulesNotSupportedByProvider);
}
