using MediatR;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptions.Abstractions;
using OnlineConsulting.Modules.Tenancy.Domain;
using OnlineConsulting.SharedKernel.Payments;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.Signup;

/// <summary>Compensates a successful charge when CreateTenantAdminCommand fails right after (rare concurrent-duplicate-email race) - cancels the just-created provider subscription so nobody pays for a tenant with no admin user, and marks the tenant Failed.</summary>
public record RollbackTenantSignupCommand(Guid TenantId) : IRequest<OperationResult>;

public class RollbackTenantSignupHandler(ITenantRepository tenantRepository, ITenantSubscriptionRepository tenantSubscriptionRepository, ISubscriptionGateway subscriptionGateway)
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
        if (tenantSubscription?.ProviderSubscriptionId is not null)
        {
            _ = await subscriptionGateway.CancelSubscriptionAsync(tenantSubscription.ProviderSubscriptionId, cancellationToken);
            tenantSubscription.Status = TenantSubscriptionStatuses.Cancelled;
            _ = await tenantSubscriptionRepository.UpdateAsync(tenantSubscription);
        }

        tenant.Status = TenantStatuses.Failed;
        _ = await tenantRepository.UpdateAsync(tenant);

        return Result.Success("Tenant signup rolled back.");
    }
}
