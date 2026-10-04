using Core.PersistenceLayer.Repositories.Entities;

namespace OnlineConsulting.Modules.Tenancy.Domain;

/// <summary>A tenant's provider subscription, made of one <see cref="TenantSubscriptionItem"/> per purchased module.</summary>
public class TenantSubscription : SequentialGuidEntity
{
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

    /// <summary>Undoes a signup: forgets the provider subscription and cancels.</summary>
    public void CancelSignup()
    {
        ProviderSubscriptionId = null;
        Status = TenantSubscriptionStatuses.Cancelled;
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
