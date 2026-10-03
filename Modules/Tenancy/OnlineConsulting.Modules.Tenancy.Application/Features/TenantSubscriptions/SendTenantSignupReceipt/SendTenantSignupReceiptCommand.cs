using MediatR;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptions.Abstractions;
using OnlineConsulting.Modules.Tenancy.Domain;
using OnlineConsulting.SharedKernel.Tenancy;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptions.SendTenantSignupReceipt;

/// <summary>Sent only once the whole signup has succeeded (admin created, owner set) - a signup that is rolled back and refunded never gets a receipt.
/// Renewals are receipted from the invoice.paid webhook instead (OnTenantSubscriptionRenewedHandler).</summary>
public record SendTenantSignupReceiptCommand(Guid TenantId) : IRequest<OperationResult>, IBypassesTenantStatusCheck;

public class SendTenantSignupReceiptHandler(ITenantRepository tenantRepository, ITenantSubscriptionRepository subscriptionRepository, TenantReceiptSender receiptSender)
    : IRequestHandler<SendTenantSignupReceiptCommand, OperationResult>
{
    public async Task<OperationResult> Handle(SendTenantSignupReceiptCommand request, CancellationToken cancellationToken)
    {
        var tenant = await tenantRepository.GetAsync(t => t.Id == request.TenantId, cancellationToken: cancellationToken);
        var subscription = tenant is null ? null : await subscriptionRepository.GetAsync(s => s.TenantId == tenant.Id, cancellationToken: cancellationToken);

        if (tenant is null || subscription is not { Status: TenantSubscriptionStatuses.Active, ProviderSubscriptionId: { } providerSubscriptionId })
        {
            return Result.Success("No receipt to send.");
        }

        await receiptSender.SendLatestAsync(tenant, providerSubscriptionId, cancellationToken);
        return Result.Success("Receipt sent.");
    }
}
