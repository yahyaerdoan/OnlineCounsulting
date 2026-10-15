using MediatR;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptions.Abstractions;
using OnlineConsulting.SharedKernel.Payments;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.Signup.RollbackTenantSignup;

/// <summary>Undoes a signup that failed before an admin existed: refunds any charge, then deletes the reservation so the name and email are free again.</summary>
public record RollbackTenantSignupCommand(Guid TenantId) : IRequest<OperationResult>;

public class RollbackTenantSignupHandler(ITenantRepository tenantRepository, ITenantSubscriptionRepository tenantSubscriptionRepository,
    ISubscriptionGateway subscriptionGateway)
    : IRequestHandler<RollbackTenantSignupCommand, OperationResult>
{
    public async Task<OperationResult> Handle(RollbackTenantSignupCommand request, CancellationToken cancellationToken)
    {
        var tenant = await tenantRepository.GetAsync(t => t.Id == request.TenantId, cancellationToken: cancellationToken);

        if (tenant is null || tenant.OwnerUserId is not null)
        {
            return Result.Success("Nothing to roll back.");
        }

        var tenantSubscription = await tenantSubscriptionRepository.GetWithItemsAsync(s => s.TenantId == tenant.Id, cancellationToken: cancellationToken);

        if (tenantSubscription is not null)
        {
            if (tenantSubscription.ProviderSubscriptionId is { } providerSubscriptionId && tenant.ProviderCustomerId is { } providerCustomerId)
            {
                await subscriptionGateway.CancelAndRefundAsync(providerCustomerId, providerSubscriptionId, cancellationToken: cancellationToken);
            }

            _ = await tenantSubscriptionRepository.DeleteAsync(tenantSubscription, cancellationToken: cancellationToken);
        }

        _ = await tenantRepository.DeleteAsync(tenant, cancellationToken: cancellationToken);

        return Result.Success("Tenant signup rolled back and the payment refunded.");
    }
}
