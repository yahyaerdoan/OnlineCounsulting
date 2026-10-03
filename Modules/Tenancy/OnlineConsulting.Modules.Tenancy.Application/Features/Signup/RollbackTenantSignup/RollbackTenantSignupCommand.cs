using MediatR;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptionItems.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptions.Abstractions;
using OnlineConsulting.Modules.Tenancy.Domain;
using OnlineConsulting.SharedKernel.Payments;
using OnlineConsulting.SharedKernel.Persistence;
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

            tenantSubscription.ProviderSubscriptionId = null;
            tenantSubscription.Status = TenantSubscriptionStatuses.Cancelled;
            _ = await tenantSubscriptionRepository.UpdateAsync(tenantSubscription);

            var items = await tenantSubscriptionItemRepository.GetListAsync(i => i.TenantSubscriptionId == tenantSubscription.Id, orderBy: q => q.OrderBy(i => i.Id),
                size: RepositoryQuerySize.Unbounded, enableTracking: true, cancellationToken: cancellationToken);
            foreach (var item in items.Items)
            {
                item.ProviderSubscriptionItemId = null;
                item.Status = TenantSubscriptionItemStatuses.Pending;
                _ = await tenantSubscriptionItemRepository.UpdateAsync(item);
            }
        }

        tenant.Status = TenantStatuses.Failed;

        _ = await tenantRepository.UpdateAsync(tenant);

        return Result.Success("Tenant signup rolled back and the payment refunded.");
    }
}
