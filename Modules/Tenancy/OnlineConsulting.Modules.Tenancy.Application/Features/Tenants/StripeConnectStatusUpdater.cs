using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Abstractions;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.Tenants;

/// <summary>Applies a Connect account's capabilities to the tenant that owns it; shared by the Connect webhook (unauthenticated, identifies the tenant by account id) and the onboarding return page's live refresh.</summary>
public class StripeConnectStatusUpdater(ITenantRepository tenantRepository)
{
    /// <summary>No-op (not an error) if no tenant has this account id - a stale/duplicate webhook delivery, or an account created but never attached to a tenant.</summary>
    public async Task ApplyAsync(string connectedAccountId, bool chargesEnabled, bool payoutsEnabled, CancellationToken cancellationToken)
    {
        var tenant = await tenantRepository.GetAsync(t => t.StripeConnectedAccountId == connectedAccountId, cancellationToken: cancellationToken);
        if (tenant is null)
        {
            return;
        }

        tenant.UpdateStripeConnectStatus(chargesEnabled, payoutsEnabled);
        _ = await tenantRepository.UpdateAsync(tenant, cancellationToken: cancellationToken);
    }
}
