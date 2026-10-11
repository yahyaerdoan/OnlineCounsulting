using MediatR;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Contracts;
using OnlineConsulting.SharedKernel.Tenancy;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.GetTenantPaymentAccount;

/// <summary>The caller's tenant's Stripe Connect account id, for client-side Stripe.js to pick the right account context. Bypasses the status
/// check so even a suspended business's pending invoices/memberships page can still mount its payment form.</summary>
public record GetTenantPaymentAccountQuery : IRequest<OperationDataResult<TenantPaymentAccountResponse>>, IBypassesTenantStatusCheck;

public class GetTenantPaymentAccountHandler(ITenantProvider tenantProvider, ITenantPaymentAccountReader paymentAccountReader)
    : IRequestHandler<GetTenantPaymentAccountQuery, OperationDataResult<TenantPaymentAccountResponse>>
{
    public async Task<OperationDataResult<TenantPaymentAccountResponse>> Handle(GetTenantPaymentAccountQuery request, CancellationToken cancellationToken)
    {
        var paymentAccount = await paymentAccountReader.GetAsync(tenantProvider.TenantId, cancellationToken);

        return Result.Success(new TenantPaymentAccountResponse(paymentAccount.ConnectedAccountId), "Payment account retrieved successfully.");
    }
}
