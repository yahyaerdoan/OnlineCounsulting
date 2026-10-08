using Core.PersistenceLayer.Repositories.Entities;

namespace OnlineConsulting.Modules.Tenancy.Domain;

/// <summary>One purchased module within a tenant's subscription; created, billed and removed only through <see cref="TenantSubscription"/>. Not tenant-scoped itself.</summary>
public class TenantSubscriptionItem : Entity<Guid>
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

    internal bool IsActive => Status == TenantSubscriptionItemStatuses.Active && DeletedDate is null;

    internal static TenantSubscriptionItem Add(Guid tenantSubscriptionId, string moduleKey, decimal price, DateTime addedAt)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(moduleKey);
        ArgumentOutOfRangeException.ThrowIfNegative(price);

        return new TenantSubscriptionItem { TenantSubscriptionId = tenantSubscriptionId, ModuleKey = moduleKey, PriceAtAddition = price, AddedAt = addedAt };
    }

    internal void Activate(string? providerSubscriptionItemId)
    {
        EnsureAwaitingBilling(nameof(Activate));
        ProviderSubscriptionItemId = providerSubscriptionItemId;
        Status = TenantSubscriptionItemStatuses.Active;
    }

    internal void MarkBillingFailed()
    {
        EnsureAwaitingBilling(nameof(MarkBillingFailed));
        Status = TenantSubscriptionItemStatuses.Failed;
    }

    internal void ResetForRetry()
    {
        ProviderSubscriptionItemId = null;
        Status = TenantSubscriptionItemStatuses.Pending;
    }

    internal void Remove(DateTimeOffset removedAt) => DeletedDate = removedAt;

    private void EnsureAwaitingBilling(string action)
    {
        if (!IsAwaitingBilling)
        {
            throw new InvalidOperationException($"{action} needs a module awaiting billing, but {Id} is {Status}.");
        }
    }
}
