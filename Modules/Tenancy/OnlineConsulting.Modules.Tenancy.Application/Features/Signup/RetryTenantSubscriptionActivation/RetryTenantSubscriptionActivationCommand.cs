using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Tenancy.Application.Features.Signup.Contracts;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptionItems.Constants;
using OnlineConsulting.SharedKernel.CurrentUser;
using OnlineConsulting.SharedKernel.Tenancy;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.Signup.RetryTenantSubscriptionActivation;

/// <summary>The tenant admin's retry of a signup whose billing failed (signup can't be resubmitted once the admin user exists). Roles => [] deliberately: authorization is an ownership check (see TenantOwnershipGuard), not a role.</summary>
public record RetryTenantSubscriptionActivationCommand(Guid TenantId, string PaymentMethodId)
    : IRequest<OperationDataResult<ActivateTenantSubscriptionResult>>, ISecureAddRequest, IBypassesTenantStatusCheck
{
    public string[] Roles => [];
}

public class RetryTenantSubscriptionActivationHandler(TenantSubscriptionActivator activator, ITenantProvider tenantProvider, ICurrentUserAccessor currentUserAccessor)
    : IRequestHandler<RetryTenantSubscriptionActivationCommand, OperationDataResult<ActivateTenantSubscriptionResult>>
{
    public async Task<OperationDataResult<ActivateTenantSubscriptionResult>> Handle(RetryTenantSubscriptionActivationCommand request, CancellationToken cancellationToken)
    {
        if (!TenantOwnershipGuard.CallerMayManage(request.TenantId, tenantProvider.TenantId, currentUserAccessor))
        {
            return Result.Forbidden<ActivateTenantSubscriptionResult>(TenantSubscriptionItemMessages.NotAuthorizedForTenant);
        }

        return await activator.ActivateAsync(request.TenantId, request.PaymentMethodId, cancellationToken);
    }
}
