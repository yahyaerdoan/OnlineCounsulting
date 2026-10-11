namespace OnlineConsulting.SharedKernel.Tenancy;

/// <summary>Cross-module read of a tenant's Stripe Connect routing; implemented by the Tenancy module. Lets Commerce/Memberships charge a customer on the owning tenant's own account instead of the platform's.</summary>
public interface ITenantPaymentAccountReader
{
    /// <summary>The tenant's payment-routing state; ConnectedAccountId is null and IsReadyForCharges is false until the tenant finishes Stripe Connect onboarding.</summary>
    Task<TenantPaymentAccount> GetAsync(Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>Resolves a tenant by its connected account id - used by the Connect webhook, which identifies the account but not the tenant.</summary>
    Task<Guid?> FindTenantIdByConnectedAccountAsync(string connectedAccountId, CancellationToken cancellationToken = default);
}
