using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Constants;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Rules;

public static class TenantBusinessRules
{
    public static OperationResult TenantNotFound() =>
        Result.NotFound(TenantMessages.TenantNotFound);

    public static OperationResult AlreadySuspended() =>
        Result.Conflict(TenantMessages.AlreadySuspended);

    public static OperationResult NotSuspendable() =>
        Result.Conflict(TenantMessages.NotSuspendable);

    public static OperationResult NotReactivatable() =>
        Result.Conflict(TenantMessages.NotReactivatable);

    public static OperationResult NotCancellable() =>
        Result.Conflict(TenantMessages.NotCancellable);

    public static OperationResult CancellationFailed() =>
        Result.BadGateway(TenantMessages.CancellationFailed);
}
