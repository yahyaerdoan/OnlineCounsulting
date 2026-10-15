using Microsoft.Extensions.Logging;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Abstractions;
using OnlineConsulting.SharedKernel.Payments;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.Tenants;

/// <summary>Not a MediatR command - the browser arrives here straight from Stripe's OAuth redirect with no bearer token, so there is no caller to authorize against;
/// state (not auth) is what proves which tenant started this and guards against a forged callback. Mirrors TenantSubscriptionActivator/StripeConnectStatusUpdater's plain-class shape.</summary>
public class StripeConnectOAuthCallbackHandler(ITenantRepository tenantRepository, IConnectAccountGateway connectAccountGateway, IStripeConnectStateProtector stateProtector,
    ILogger<StripeConnectOAuthCallbackHandler> logger)
{
    /// <summary>Null means the state was missing/tampered/expired or the tenant no longer exists - the caller should show a generic error, not redirect anywhere tenant-specific.</summary>
    public async Task<string?> HandleAsync(string? code, string? state, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(state) || stateProtector.Unprotect(state) is not var (tenantId, returnUrl))
        {
            return null;
        }

        var tenant = await tenantRepository.GetAsync(t => t.Id == tenantId, cancellationToken: cancellationToken);
        if (tenant is null)
        {
            return null;
        }

        try
        {
            var account = await connectAccountGateway.ExchangeOAuthCodeAsync(code, cancellationToken);

            tenant.ConnectStripeAccount(account.AccountId);
            _ = await tenantRepository.UpdateAsync(tenant, cancellationToken: cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Tenant {TenantId}: exchanging the Stripe Connect OAuth code failed.", tenant.Id);
        }

        return returnUrl;
    }
}
