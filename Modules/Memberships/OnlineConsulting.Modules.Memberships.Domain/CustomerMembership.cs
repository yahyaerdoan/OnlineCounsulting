using Core.PersistenceLayer.MultiTenancy;

namespace OnlineConsulting.Modules.Memberships.Domain;

/// <summary>A customer's subscription to a membership plan. State changes only through its methods, which throw when called in the wrong state.</summary>
public class CustomerMembership : SequentialGuidTenantEntity
{
    private CustomerMembership()
    {
    }

    /// <summary>Identity module user id, no navigation.</summary>
    public Guid UserId { get; private set; }

    public Guid MembershipPlanId { get; private set; }

    /// <summary>One of <see cref="CustomerMembershipStatuses"/>.</summary>
    public string Status { get; private set; } = CustomerMembershipStatuses.PendingPayment;

    public DateTimeOffset StartDate { get; private set; }
    public DateTimeOffset? RenewalDate { get; private set; }

    public string? ProviderCustomerId { get; private set; }
    public string? ProviderSubscriptionId { get; private set; }

    /// <summary>Cancellation requested; the status stays as is until the period ends.</summary>
    public bool CancelAtPeriodEnd { get; private set; }

    /// <summary>When the membership first became past due; drives the grace-period cleanup.</summary>
    public DateTimeOffset? PastDueSince { get; private set; }

    /// <summary>End of the plan's free trial, set at subscribe time.</summary>
    public DateTimeOffset? TrialEndDate { get; private set; }

    /// <summary>Promo code redeemed at subscribe time; blocks redeeming it again.</summary>
    public Guid? PromoCodeId { get; private set; }

    /// <summary>Ended for good.</summary>
    public bool IsCancelled => CustomerMembershipRules.IsCancelled(Status);

    /// <summary>Signup not finished: the first payment is pending or the attempt failed.</summary>
    public bool IsAwaitingFirstPayment => CustomerMembershipRules.IsAwaitingFirstPayment(Status);

    /// <summary>Active, so it can be paused.</summary>
    public bool CanBePaused => CustomerMembershipRules.CanBePaused(Status);

    /// <summary>Paused, so it can be resumed.</summary>
    public bool CanBeResumed => CustomerMembershipRules.CanBeResumed(Status);

    /// <summary>Active and not set to end.</summary>
    public bool CanChangePlan => CustomerMembershipRules.CanChangePlan(Status, CancelAtPeriodEnd);

    /// <summary>Not cancelled and not already set to end.</summary>
    public bool CanBeCancelledAtPeriodEnd => CustomerMembershipRules.CanBeCancelledAtPeriodEnd(Status, CancelAtPeriodEnd);

    /// <summary>Set to end but not cancelled yet.</summary>
    public bool CanBeReactivated => CustomerMembershipRules.CanBeReactivated(Status, CancelAtPeriodEnd);

    /// <summary>Starts a signup awaiting its first payment.</summary>
    public static CustomerMembership Start(Guid userId, Guid membershipPlanId, DateTimeOffset startDate) => new()
    {
        UserId = userId,
        MembershipPlanId = membershipPlanId,
        StartDate = startDate,
    };

    /// <summary>Moves an unfinished signup without a subscription to another plan.</summary>
    public void SwitchPendingPlan(Guid membershipPlanId)
    {
        EnsureSignupWithoutSubscription(nameof(SwitchPendingPlan));
        MembershipPlanId = membershipPlanId;
    }

    /// <summary>Remembers the payment provider's customer record.</summary>
    public void LinkProviderCustomer(string providerCustomerId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(providerCustomerId);
        ProviderCustomerId = providerCustomerId;
    }

    /// <summary>Records the subscription created for an unfinished signup; the status is set by <see cref="Activate"/> or <see cref="MarkPaymentFailed"/>.</summary>
    public void AttachSubscription(string providerSubscriptionId, DateTimeOffset? currentPeriodEnd, DateTimeOffset? trialEndDate, Guid? promoCodeId)
    {
        EnsureSignupWithoutSubscription(nameof(AttachSubscription));

        ArgumentException.ThrowIfNullOrWhiteSpace(providerSubscriptionId);

        ProviderSubscriptionId = providerSubscriptionId;
        RenewalDate = currentPeriodEnd;
        TrialEndDate = trialEndDate;
        PromoCodeId = promoCodeId;
    }

    /// <summary>Completes a signup once its first payment succeeds. Requires <see cref="IsAwaitingFirstPayment"/>.</summary>
    public void Activate()
    {
        if (!IsAwaitingFirstPayment)
        {
            throw new InvalidOperationException($"Only an unfinished signup can be activated, but membership {Id} is {Status}.");
        }

        Status = CustomerMembershipStatuses.Active;
    }

    /// <summary>Marks the signup attempt as failed so it can be retried.</summary>
    public void MarkSignupFailed()
    {
        EnsureNotCancelled(nameof(MarkSignupFailed));
        Status = CustomerMembershipStatuses.Failed;
    }

    /// <summary>A paid period started: active again until <paramref name="currentPeriodEnd"/>.</summary>
    public void Renew(DateTimeOffset? currentPeriodEnd)
    {
        EnsureNotCancelled(nameof(Renew));
        RenewalDate = currentPeriodEnd;
        Status = CustomerMembershipStatuses.Active;
        PastDueSince = null;
    }

    /// <summary>A payment failed: past due, keeping the first time it happened.</summary>
    public void MarkPaymentFailed(DateTimeOffset failedAt)
    {
        EnsureNotCancelled(nameof(MarkPaymentFailed));
        Status = CustomerMembershipStatuses.PastDue;
        PastDueSince ??= failedAt;
    }

    /// <summary>Requires <see cref="CanBePaused"/>.</summary>
    public void Pause()
    {
        if (!CanBePaused)
        {
            throw new InvalidOperationException($"Only an active membership can be paused, but {Id} is {Status}.");
        }

        Status = CustomerMembershipStatuses.Paused;
    }

    /// <summary>Requires <see cref="CanBeResumed"/>.</summary>
    public void Resume()
    {
        if (!CanBeResumed)
        {
            throw new InvalidOperationException($"Only a paused membership can be resumed, but {Id} is {Status}.");
        }

        Status = CustomerMembershipStatuses.Active;
    }

    /// <summary>Requires <see cref="CanChangePlan"/> and a different plan.</summary>
    public void ChangePlan(Guid membershipPlanId)
    {
        if (!CanChangePlan)
        {
            throw new InvalidOperationException($"Only an active membership that isn't ending can change plans, but {Id} is {Status}.");
        }

        if (membershipPlanId == MembershipPlanId)
        {
            throw new InvalidOperationException($"Membership {Id} is already on plan {membershipPlanId}.");
        }

        MembershipPlanId = membershipPlanId;
    }

    /// <summary>Ends the membership when the current period ends. Requires <see cref="CanBeCancelledAtPeriodEnd"/>.</summary>
    public void CancelAtEndOfPeriod()
    {
        if (!CanBeCancelledAtPeriodEnd)
        {
            throw new InvalidOperationException($"Membership {Id} is already cancelled or set to end.");
        }

        CancelAtPeriodEnd = true;
    }

    /// <summary>Undoes <see cref="CancelAtEndOfPeriod"/>. Requires <see cref="CanBeReactivated"/>.</summary>
    public void Reactivate()
    {
        if (!CanBeReactivated)
        {
            throw new InvalidOperationException($"Membership {Id} isn't set to end, so it cannot be reactivated.");
        }

        CancelAtPeriodEnd = false;
    }

    /// <summary>Ends the membership now.</summary>
    public void Cancel()
    {
        EnsureNotCancelled(nameof(Cancel));
        Status = CustomerMembershipStatuses.Cancelled;
        PastDueSince = null;
    }

    private void EnsureSignupWithoutSubscription(string action)
    {
        if (!IsAwaitingFirstPayment || ProviderSubscriptionId is not null)
        {
            throw new InvalidOperationException($"{action} needs an unfinished signup without a subscription, but membership {Id} is {Status}.");
        }
    }

    private void EnsureNotCancelled(string action)
    {
        if (IsCancelled)
        {
            throw new InvalidOperationException($"{action} cannot run on cancelled membership {Id}.");
        }
    }
}
