using Core.PersistenceLayer.Repositories.Entities;

namespace OnlineConsulting.Modules.Tenancy.Domain;

/// <summary>
/// A tenant's provider subscription and the aggregate root of its modules (<see cref="Items"/>): modules are added, billed and removed only through
/// its methods, which keep one live item per module and at least one active module. Load it with its items and save them together.
/// </summary>
public class TenantSubscription : SequentialGuidEntity
{
    private readonly List<TenantSubscriptionItem> _items = [];

    private TenantSubscription()
    {
    }

    public Guid TenantId { get; private set; }

    /// <summary>One of <see cref="TenantSubscriptionStatuses"/>.</summary>
    public string Status { get; private set; } = TenantSubscriptionStatuses.PendingPayment;

    public DateTime StartDate { get; private set; }
    public DateTime? RenewalDate { get; private set; }
    public string? ProviderSubscriptionId { get; private set; }

    public bool IsCancelled => Status == TenantSubscriptionStatuses.Cancelled;

    /// <summary>The modules not removed yet, in any billing state.</summary>
    public IReadOnlyList<TenantSubscriptionItem> Items => [.. _items.Where(i => i.DeletedDate is null)];

    /// <summary>The billed modules the tenant can use.</summary>
    public IReadOnlyList<TenantSubscriptionItem> ActiveItems => [.. _items.Where(i => i.IsActive)];

    public bool HasActiveModule(string moduleKey) => _items.Any(i => i.IsActive && i.ModuleKey == moduleKey);

    /// <summary>The module is active and isn't the last one, so it can be removed.</summary>
    public bool CanRemoveModule(string moduleKey) => HasActiveModule(moduleKey) && ActiveItems.Count > 1;

    /// <summary>Starts a subscription awaiting its first payment.</summary>
    public static TenantSubscription Start(Guid tenantId, DateTime startDate) => new() { TenantId = tenantId, StartDate = startDate };

    /// <summary>Puts a retried signup back to awaiting payment; only before the provider subscription exists.</summary>
    public void RestartSignup()
    {
        EnsureNoProviderSubscription(nameof(RestartSignup));
        Status = TenantSubscriptionStatuses.PendingPayment;
    }

    /// <summary>Records the provider subscription; active when its first payment already succeeded.</summary>
    public void AttachProviderSubscription(string providerSubscriptionId, DateTime renewalDate, bool paid)
    {
        EnsureNoProviderSubscription(nameof(AttachProviderSubscription));
        ArgumentException.ThrowIfNullOrWhiteSpace(providerSubscriptionId);
        ProviderSubscriptionId = providerSubscriptionId;
        RenewalDate = renewalDate;
        Status = paid ? TenantSubscriptionStatuses.Active : TenantSubscriptionStatuses.PendingPayment;
    }

    /// <summary>Marks the signup as failed so it can be retried.</summary>
    public void MarkFailed()
    {
        EnsureNotCancelled(nameof(MarkFailed));
        Status = TenantSubscriptionStatuses.Failed;
    }

    /// <summary>A retried signup whose provider subscription already exists becomes active again.</summary>
    public void RecoverFromFailure()
    {
        if (Status != TenantSubscriptionStatuses.Failed || ProviderSubscriptionId is null)
        {
            throw new InvalidOperationException($"Only a failed subscription with a provider subscription can recover, but {Id} is {Status}.");
        }

        Status = TenantSubscriptionStatuses.Active;
    }

    /// <summary>Undoes a signup: forgets the provider subscription, cancels and puts every module back to awaiting billing.</summary>
    public void CancelSignup()
    {
        ProviderSubscriptionId = null;
        Status = TenantSubscriptionStatuses.Cancelled;

        foreach (var item in Items)
        {
            item.ResetForRetry();
        }
    }

    public void Cancel()
    {
        EnsureNotCancelled(nameof(Cancel));
        Status = TenantSubscriptionStatuses.Cancelled;
    }

    public void MarkPastDue()
    {
        EnsureNotCancelled(nameof(MarkPastDue));
        Status = TenantSubscriptionStatuses.PastDue;
    }

    /// <summary>A paid period started: active until <paramref name="renewalDate"/>.</summary>
    public void Renew(DateTime renewalDate)
    {
        EnsureNotCancelled(nameof(Renew));
        RenewalDate = renewalDate;
        Status = TenantSubscriptionStatuses.Active;
    }

    /// <summary>Records the modules chosen at signup: adds the missing ones and, until the provider subscription exists, drops the ones no longer chosen.</summary>
    public void SelectSignupModules(IReadOnlyDictionary<string, decimal> pricesByModuleKey, DateTimeOffset now)
    {
        if (ProviderSubscriptionId is null)
        {
            foreach (var dropped in Items.Where(i => !pricesByModuleKey.ContainsKey(i.ModuleKey)))
            {
                dropped.Remove(now);
            }
        }

        var recorded = Items.Select(i => i.ModuleKey).ToHashSet();
        foreach (var (moduleKey, price) in pricesByModuleKey.Where(p => !recorded.Contains(p.Key)))
        {
            _items.Add(TenantSubscriptionItem.Add(Id, moduleKey, price, now.UtcDateTime));
        }
    }

    /// <summary>The module's item awaiting billing: the one a failed attempt left, or a new one. Throws when the module is already active.</summary>
    public TenantSubscriptionItem AddModule(string moduleKey, decimal price, DateTimeOffset now)
    {
        if (HasActiveModule(moduleKey))
        {
            throw new InvalidOperationException($"Module {moduleKey} is already active on subscription {Id}.");
        }

        if (Items.FirstOrDefault(i => i.ModuleKey == moduleKey && i.IsAwaitingBilling) is { } awaiting)
        {
            return awaiting;
        }

        var item = TenantSubscriptionItem.Add(Id, moduleKey, price, now.UtcDateTime);
        _items.Add(item);
        return item;
    }

    /// <summary>The module is billed; the provider item id is null for providers without separate item ids.</summary>
    public void ActivateModule(string moduleKey, string? providerSubscriptionItemId) => GetItem(moduleKey).Activate(providerSubscriptionItemId);

    /// <summary>Billing the module failed; adding it again retries the same item.</summary>
    public void FailModuleBilling(string moduleKey) => GetItem(moduleKey).MarkBillingFailed();

    /// <summary>Removes an active module, keeping its row as history. Requires <see cref="CanRemoveModule"/>.</summary>
    public void RemoveModule(string moduleKey, DateTimeOffset now)
    {
        if (!CanRemoveModule(moduleKey))
        {
            throw new InvalidOperationException($"Module {moduleKey} cannot be removed from subscription {Id}: it isn't active or it's the last module.");
        }

        ActiveItems.First(i => i.ModuleKey == moduleKey).Remove(now);
    }

    private TenantSubscriptionItem GetItem(string moduleKey) =>
        Items.FirstOrDefault(i => i.ModuleKey == moduleKey) ?? throw new InvalidOperationException($"Subscription {Id} has no module {moduleKey}.");

    private void EnsureNoProviderSubscription(string action)
    {
        if (ProviderSubscriptionId is not null)
        {
            throw new InvalidOperationException($"{action} needs a subscription without a provider subscription, but {Id} has {ProviderSubscriptionId}.");
        }
    }

    private void EnsureNotCancelled(string action)
    {
        if (IsCancelled)
        {
            throw new InvalidOperationException($"{action} cannot run on cancelled subscription {Id}.");
        }
    }
}
