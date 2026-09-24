using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Constants;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Rules;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptions.Abstractions;
using OnlineConsulting.Modules.Tenancy.Domain;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.Payments;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.CancelTenant;

/// <summary>Platform-owner offboarding - permanent, unlike Suspend/Reactivate. Cancels the tenant's provider subscription and marks tenant+subscription Cancelled; no data is deleted.</summary>
public record CancelTenantCommand(Guid TenantId) : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [GlobalOperationClaims.SuperAdmin];

    /// <summary>Cross-tenant/platform-level - a tenant admin must never reach this, even with TenantFullAccess.</summary>
    [JsonIgnore]
    public bool AllowTenantBypass => false;
}

public class CancelTenantHandler(ITenantRepository tenantRepository, ITenantSubscriptionRepository tenantSubscriptionRepository, ISubscriptionGateway subscriptionGateway)
    : IRequestHandler<CancelTenantCommand, OperationResult>
{
    public async Task<OperationResult> Handle(CancelTenantCommand request, CancellationToken cancellationToken)
    {
        var tenant = await tenantRepository.GetAsync(t => t.Id == request.TenantId, cancellationToken: cancellationToken);

        if (tenant is null)
        {
            return TenantBusinessRules.TenantNotFound();
        }

        if (tenant.Status == TenantStatuses.Cancelled)
        {
            return TenantBusinessRules.NotCancellable();
        }

        var subscription = await tenantSubscriptionRepository.GetAsync(s => s.TenantId == request.TenantId && s.Status != TenantSubscriptionStatuses.Cancelled, cancellationToken: cancellationToken);

        if (subscription?.ProviderSubscriptionId is { } providerSubscriptionId)
        {
            var failure = await PaymentGatewayCall.RunAsync(() => subscriptionGateway.CancelSubscriptionAsync(providerSubscriptionId, cancellationToken: cancellationToken), TenantMessages.CancellationFailed);

            if (failure is not null)
            {
                return failure;
            }

            subscription.Status = TenantSubscriptionStatuses.Cancelled;

            _ = await tenantSubscriptionRepository.UpdateAsync(subscription);
        }

        tenant.Status = TenantStatuses.Cancelled;

        _ = await tenantRepository.UpdateAsync(tenant);

        return Result.Success("Tenant cancelled.");
    }
}
