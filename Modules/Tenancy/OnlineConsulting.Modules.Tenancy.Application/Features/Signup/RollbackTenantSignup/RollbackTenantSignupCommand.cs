using MediatR;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptionItems.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptions.Abstractions;
using OnlineConsulting.Modules.Tenancy.Domain;
using OnlineConsulting.SharedKernel.Payments;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.Signup.RollbackTenantSignup;

/// <summary>Compensates a paid signup whose admin account couldn't be created: cancels the provider subscription, refunds the payment, and
/// resets the reservation (no provider ids, items back to Pending) so a retry with the same email starts a fresh subscription instead of
/// reaching for the cancelled one.</summary>
public record RollbackTenantSignupCommand(Guid TenantId) : IRequest<OperationResult>;

public class RollbackTenantSignupHandler(ITenantRepository tenantRepository, ITenantSubscriptionRepository tenantSubscriptionRepository,
    ITenantSubscriptionItemRepository tenantSubscriptionItemRepository, ISubscriptionGateway subscriptionGateway)
    : IRequestHandler<RollbackTenantSignupCommand, OperationResult>
{
    public async Task<OperationResult> Handle(RollbackTenantSignupCommand request, CancellationToken cancellationToken)
    {
        var tenant = await tenantRepository.GetAsync(t => t.Id == request.TenantId, cancellationToken: cancellationToken);

        if (tenant is null)
        {
            return Result.Success("Nothing to roll back.");
        }

        var tenantSubscription = await tenantSubscriptionRepository.GetAsync(s => s.TenantId == tenant.Id, cancellationToken: cancellationToken);

        if (tenantSubscription is not null)
        {
            if (tenantSubscription.ProviderSubscriptionId is { } providerSubscriptionId && tenant.ProviderCustomerId is { } providerCustomerId)
            {
                await subscriptionGateway.CancelAndRefundAsync(providerCustomerId, providerSubscriptionId, cancellationToken);
            }

            tenantSubscription.CancelSignup();
            _ = await tenantSubscriptionRepository.UpdateAsync(tenantSubscription, cancellationToken: cancellationToken);

            var items = await tenantSubscriptionItemRepository.GetAllAsync(i => i.TenantSubscriptionId == tenantSubscription.Id, enableTracking: true, cancellationToken: cancellationToken);
            foreach (var item in items)
            {
                item.ResetForRetry();
                _ = await tenantSubscriptionItemRepository.UpdateAsync(item, cancellationToken: cancellationToken);
            }
        }

        tenant.FailSignup();

        _ = await tenantRepository.UpdateAsync(tenant, cancellationToken: cancellationToken);

        return Result.Success("Tenant signup rolled back and the payment refunded.");
    }
}
