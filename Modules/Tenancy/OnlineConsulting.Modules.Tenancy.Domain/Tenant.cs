using Core.PersistenceLayer.Repositories.Entities;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Tenancy.Domain;

/// <summary>A paying customer organization; not tenant-scoped itself. State changes only through its methods, which throw when called in the wrong state.</summary>
public class Tenant : SequentialGuidEntity
{
    private Tenant()
    {
    }

    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;

    /// <summary>One of <see cref="TenantStatuses"/>.</summary>
    public string Status { get; private set; } = TenantStatuses.PendingPayment;

    public string PrimaryContactEmail { get; private set; } = string.Empty;

    /// <summary>Payment provider customer id for the tenant's billing.</summary>
    public string? ProviderCustomerId { get; private set; }

    /// <summary>The first admin, who gets owner-only protections; null for tenants created before owners were recorded.</summary>
    public Guid? OwnerUserId { get; private set; }

    /// <summary>IANA zone the business runs in (e.g. "America/Chicago"); appointments are scheduled and shown in it.</summary>
    public string TimeZoneId { get; private set; } = BusinessTimeZones.Default;

    /// <summary>Logo shown on the tenant's site and app; null shows the name alone.</summary>
    public Guid? LogoMediaAssetId { get; private set; }

    /// <summary>Stripe Connect Express account that customer-facing charges (orders, memberships) route to; null until the tenant connects one. Distinct from <see cref="ProviderCustomerId"/>, which is the tenant's own billing identity on the platform's account.</summary>
    public string? StripeConnectedAccountId { get; private set; }

    /// <summary>Mirrors the connected account's "charges_enabled" capability; false until Stripe finishes onboarding review.</summary>
    public bool StripeChargesEnabled { get; private set; }

    /// <summary>Mirrors the connected account's "payouts_enabled" capability.</summary>
    public bool StripePayoutsEnabled { get; private set; }

    /// <summary>Whether a checkout/subscription for this tenant's own customers can be charged - everything is in place on the provider side.</summary>
    public bool IsReadyToAcceptPayments => StripeConnectedAccountId is not null && StripeChargesEnabled;

    /// <summary>Signup not finished: payment pending or the attempt failed.</summary>
    public bool IsAwaitingSignup => TenantRules.IsAwaitingSignup(Status);

    /// <summary>Suspended or cancelled by staff; billing events don't change it.</summary>
    public bool IsHeldByStaff => TenantRules.IsHeldByStaff(Status);

    /// <summary>Active or past due.</summary>
    public bool CanBeSuspended => TenantRules.CanBeSuspended(Status);

    /// <summary>Suspended.</summary>
    public bool CanBeReactivated => TenantRules.CanBeReactivated(Status);

    /// <summary>Not cancelled yet.</summary>
    public bool CanBeCancelled => TenantRules.CanBeCancelled(Status);

    /// <summary>Starts a signup awaiting payment.</summary>
    public static Tenant Reserve(string name, string slug, string primaryContactEmail)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(slug);
        ArgumentException.ThrowIfNullOrWhiteSpace(primaryContactEmail);

        return new Tenant { Name = name, Slug = slug, PrimaryContactEmail = primaryContactEmail };
    }

    /// <summary>Remembers the payment provider's customer record.</summary>
    public void LinkProviderCustomer(string providerCustomerId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(providerCustomerId);
        ProviderCustomerId = providerCustomerId;
    }

    /// <summary>Sets the business's time zone; must be a known IANA id.</summary>
    public void ChangeTimeZone(string timeZoneId)
    {
        if (!BusinessTimeZones.IsKnown(timeZoneId))
        {
            throw new ArgumentException($"Unknown time zone '{timeZoneId}'.", nameof(timeZoneId));
        }

        TimeZoneId = timeZoneId;
    }

    /// <summary>Sets the name and logo customers see on the tenant's site, app and emails.</summary>
    public void Rebrand(string name, Guid? logoMediaAssetId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name.Trim();
        LogoMediaAssetId = logoMediaAssetId;
    }

    /// <summary>Remembers the Stripe Connect account created (or reused) for this tenant; capabilities start false until Stripe reports otherwise.</summary>
    public void ConnectStripeAccount(string connectedAccountId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectedAccountId);
        StripeConnectedAccountId = connectedAccountId;
    }

    /// <summary>Applies the connected account's current capabilities - called from the onboarding return-page refresh and the Connect webhook.</summary>
    public void UpdateStripeConnectStatus(bool chargesEnabled, bool payoutsEnabled)
    {
        StripeChargesEnabled = chargesEnabled;
        StripePayoutsEnabled = payoutsEnabled;
    }

    /// <summary>Records the tenant's first admin as its owner.</summary>
    public void AssignOwner(Guid ownerUserId) => OwnerUserId = ownerUserId;

    /// <summary>Takes the signup result from the subscription's status. Requires the tenant not to be <see cref="IsHeldByStaff"/>.</summary>
    public void CompleteSignup(string subscriptionStatus)
    {
        EnsureFollowsBilling(nameof(CompleteSignup));
        Status = subscriptionStatus switch
        {
            TenantSubscriptionStatuses.Active => TenantStatuses.Active,
            TenantSubscriptionStatuses.PastDue => TenantStatuses.PastDue,
            _ => TenantStatuses.PendingPayment,
        };
    }

    /// <summary>Marks the signup as failed so it can be retried.</summary>
    public void FailSignup()
    {
        if (Status == TenantStatuses.Cancelled)
        {
            throw new InvalidOperationException($"Tenant {Id} is cancelled, so its signup cannot fail.");
        }

        Status = TenantStatuses.Failed;
    }

    /// <summary>A subscription payment failed. Requires the tenant not to be <see cref="IsHeldByStaff"/>.</summary>
    public void ApplyPaymentFailed()
    {
        EnsureFollowsBilling(nameof(ApplyPaymentFailed));
        Status = TenantStatuses.PastDue;
    }

    /// <summary>A subscription period was paid. Requires the tenant not to be <see cref="IsHeldByStaff"/>.</summary>
    public void ApplyRenewal()
    {
        EnsureFollowsBilling(nameof(ApplyRenewal));
        Status = TenantStatuses.Active;
    }

    /// <summary>The provider ended the subscription. Requires the tenant not to be <see cref="IsHeldByStaff"/>.</summary>
    public void ApplySubscriptionEnded()
    {
        EnsureFollowsBilling(nameof(ApplySubscriptionEnded));
        Status = TenantStatuses.Suspended;
    }

    /// <summary>Requires <see cref="CanBeSuspended"/>.</summary>
    public void Suspend()
    {
        if (!CanBeSuspended)
        {
            throw new InvalidOperationException($"Only an active or past-due tenant can be suspended, but {Id} is {Status}.");
        }

        Status = TenantStatuses.Suspended;
    }

    /// <summary>Requires <see cref="CanBeReactivated"/>.</summary>
    public void Reactivate()
    {
        if (!CanBeReactivated)
        {
            throw new InvalidOperationException($"Only a suspended tenant can be reactivated, but {Id} is {Status}.");
        }

        Status = TenantStatuses.Active;
    }

    /// <summary>Requires <see cref="CanBeCancelled"/>.</summary>
    public void Cancel()
    {
        if (!CanBeCancelled)
        {
            throw new InvalidOperationException($"Tenant {Id} is already cancelled.");
        }

        Status = TenantStatuses.Cancelled;
    }

    private void EnsureFollowsBilling(string action)
    {
        if (IsHeldByStaff)
        {
            throw new InvalidOperationException($"{action} cannot change tenant {Id} while staff hold it as {Status}.");
        }
    }
}
