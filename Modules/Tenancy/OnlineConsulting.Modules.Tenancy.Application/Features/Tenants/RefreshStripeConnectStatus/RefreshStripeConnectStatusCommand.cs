using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Pipelines.Loggings.Abstractions;
using Core.SecurityLayer.Constants;
using MediatR;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Constants;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Contracts;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.Payments;
using OnlineConsulting.SharedKernel.Tenancy;
using OnlineConsulting.SharedKernel.Transactions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.RefreshStripeConnectStatus;

/// <summary>Pulls the connected account's live capabilities right after the admin returns from Stripe onboarding, so the page doesn't have to wait for the Connect webhook to arrive.</summary>
public record RefreshStripeConnectStatusCommand : IRequest<OperationDataResult<StripeConnectStatusResponse>>, ISecureAddRequest, ITenancyTransactionRequest, ILogResultRequest
{
    public string[] Roles => [GeneralOperationClaims.Admin, GlobalOperationClaims.SuperAdmin];
}

public class RefreshStripeConnectStatusHandler(ITenantRepository tenantRepository, ITenantProvider tenantProvider, IConnectAccountGateway connectAccountGateway)
    : IRequestHandler<RefreshStripeConnectStatusCommand, OperationDataResult<StripeConnectStatusResponse>>
{
    public async Task<OperationDataResult<StripeConnectStatusResponse>> Handle(RefreshStripeConnectStatusCommand request, CancellationToken cancellationToken)
    {
        var tenant = await tenantRepository.GetAsync(t => t.Id == tenantProvider.TenantId, cancellationToken: cancellationToken);
        if (tenant is null)
        {
            return Result.NotFound<StripeConnectStatusResponse>(TenantMessages.TenantNotFound);
        }

        if (tenant.StripeConnectedAccountId is not { } accountId)
        {
            return Result.Success(new StripeConnectStatusResponse(false, false, false), "No Stripe account connected yet.");
        }

        try
        {
            var status = await connectAccountGateway.GetAccountStatusAsync(accountId, cancellationToken);
            tenant.UpdateStripeConnectStatus(status.ChargesEnabled, status.PayoutsEnabled);
            _ = await tenantRepository.UpdateAsync(tenant, cancellationToken: cancellationToken);

            return Result.Success(new StripeConnectStatusResponse(true, status.ChargesEnabled, status.PayoutsEnabled), "Stripe status refreshed.");
        }
        catch (Exception)
        {
            return Result.BadGateway<StripeConnectStatusResponse>(TenantMessages.StripeConnectSetupFailed);
        }
    }
}
