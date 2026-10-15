using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.Tenancy.Infrastructure.Persistence;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Tenancy.Infrastructure.StripeConnect;

/// <summary>Cross-module implementation of ITenantPaymentAccountReader; reads straight from the context since Tenant isn't tenant-scoped (no filter to bypass).</summary>
public class TenantPaymentAccountReader(TenancyDbContext context) : ITenantPaymentAccountReader
{
    public async Task<TenantPaymentAccount> GetAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        if (tenantId == TenantDefaults.DefaultTenantId)
        {
            return new TenantPaymentAccount(null, true);
        }

        var tenant = await context.Tenants.AsNoTracking()
            .Where(t => t.Id == tenantId)
            .Select(t => new { t.StripeConnectedAccountId, t.StripeChargesEnabled })
            .FirstOrDefaultAsync(cancellationToken);

        return tenant is null
            ? new TenantPaymentAccount(null, false)
            : new TenantPaymentAccount(tenant.StripeConnectedAccountId, tenant.StripeConnectedAccountId is not null && tenant.StripeChargesEnabled);
    }

    public async Task<Guid?> FindTenantIdByConnectedAccountAsync(string connectedAccountId, CancellationToken cancellationToken = default)
    {
        var tenant = await context.Tenants.AsNoTracking()
            .Where(t => t.StripeConnectedAccountId == connectedAccountId)
            .Select(t => new { t.Id })
            .FirstOrDefaultAsync(cancellationToken);

        return tenant?.Id;
    }
}
