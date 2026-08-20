using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptionItems.Constants;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptionItems.Rules;

public static class TenantSubscriptionItemBusinessRules
{
    public static OperationResult NotAuthorizedForTenant() =>
        Result.Forbidden(TenantSubscriptionItemMessages.NotAuthorizedForTenant);

    public static OperationResult NoActiveSubscription() =>
        Result.BadRequest(TenantSubscriptionItemMessages.NoActiveSubscription);

    public static OperationResult TenantNotFound() =>
        Result.NotFound(TenantSubscriptionItemMessages.TenantNotFound);

    public static OperationResult ModuleNotFound() =>
        Result.BadRequest(TenantSubscriptionItemMessages.ModuleNotFound);

    public static OperationResult ModuleAlreadyAdded() =>
        Result.Conflict(TenantSubscriptionItemMessages.ModuleAlreadyAdded);

    public static OperationResult ModuleNotActive() =>
        Result.NotFound(TenantSubscriptionItemMessages.ModuleNotActive);

    public static OperationResult MultipleModulesNotSupportedByProvider() =>
        Result.BadRequest(TenantSubscriptionItemMessages.MultipleModulesNotSupportedByProvider);
}
