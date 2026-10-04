using Core.PersistenceLayer.Repositories.Entities;

namespace OnlineConsulting.Modules.Tenancy.Domain;

/// <summary>One purchased module within a tenant's subscription; not tenant-scoped itself.</summary>
public class TenantSubscriptionItem : SequentialGuidEntity
{
    private TenantSubscriptionItem()
    {
    }

    public Guid TenantSubscriptionId { get; private set; }

    /// <summary>A FeatureFlagKeys value, matching ModuleOffering.Key.</summary>
    public string ModuleKey { get; private set; } = string.Empty;

    /// <summary>One of <see cref="TenantSubscriptionItemStatuses"/>.</summary>
    public string Status { get; private set; } = TenantSubscriptionItemStatuses.Pending;

    /// <summary>Provider subscription item id; adding and removing the module goes through it.</summary>
    public string? ProviderSubscriptionItemId { get; private set; }

    /// <summary>Module price when it was added, kept even if the offering's price changes.</summary>
    public decimal PriceAtAddition { get; private set; }

    public DateTime AddedAt { get; private set; }

    /// <summary>Pending or failed, so billing can be (re)tried.</summary>
    public bool IsAwaitingBilling => Status is TenantSubscriptionItemStatuses.Pending or TenantSubscriptionItemStatuses.Failed;

    /// <summary>Adds a module awaiting billing.</summary>
    public static TenantSubscriptionItem Add(Guid tenantSubscriptionId, string moduleKey, decimal price, DateTime addedAt)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(moduleKey);
        ArgumentOutOfRangeException.ThrowIfNegative(price);

        return new TenantSubscriptionItem { TenantSubscriptionId = tenantSubscriptionId, ModuleKey = moduleKey, PriceAtAddition = price, AddedAt = addedAt };
    }

    /// <summary>The module is billed; the item id is null for providers without separate item ids. Requires <see cref="IsAwaitingBilling"/>.</summary>
    public void Activate(string? providerSubscriptionItemId)
    {
        EnsureAwaitingBilling(nameof(Activate));
        ProviderSubscriptionItemId = providerSubscriptionItemId;
        Status = TenantSubscriptionItemStatuses.Active;
    }

    /// <summary>Billing failed; a retry resumes it. Requires <see cref="IsAwaitingBilling"/>.</summary>
    public void MarkBillingFailed()
    {
        EnsureAwaitingBilling(nameof(MarkBillingFailed));
        Status = TenantSubscriptionItemStatuses.Failed;
    }

    /// <summary>Undoes billing after a rolled-back signup.</summary>
    public void ResetForRetry()
    {
        ProviderSubscriptionItemId = null;
        Status = TenantSubscriptionItemStatuses.Pending;
    }

    private void EnsureAwaitingBilling(string action)
    {
        if (!IsAwaitingBilling)
        {
            throw new InvalidOperationException($"{action} needs a module awaiting billing, but {Id} is {Status}.");
        }
    }
}
