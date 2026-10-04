using MediatR;
using OnlineConsulting.Modules.Tenancy.Application.Features.Signup.Contracts;
using OnlineConsulting.SharedKernel.Tenancy;
using ResultHandler.Core.Base;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.Signup.ActivateTenantSubscription;

/// <summary>Bills the Pending items via ISubscriptionGateway; runs before CreateTenantAdminCommand (pay-first) with no caller to check, so only the signup flow sends it; the owner's retry is RetryTenantSubscriptionActivationCommand. Not ITenancyTransactionRequest - it charges a real card mid-handler, so a rollback-on-throw would strand a captured charge.</summary>
public record ActivateTenantSubscriptionCommand(Guid TenantId, string PaymentMethodId)
    : IRequest<OperationDataResult<ActivateTenantSubscriptionResult>>, IBypassesTenantStatusCheck;

public class ActivateTenantSubscriptionHandler(TenantSubscriptionActivator activator)
    : IRequestHandler<ActivateTenantSubscriptionCommand, OperationDataResult<ActivateTenantSubscriptionResult>>
{
    public Task<OperationDataResult<ActivateTenantSubscriptionResult>> Handle(ActivateTenantSubscriptionCommand request, CancellationToken cancellationToken) =>
        activator.ActivateAsync(request.TenantId, request.PaymentMethodId, cancellationToken);
}
