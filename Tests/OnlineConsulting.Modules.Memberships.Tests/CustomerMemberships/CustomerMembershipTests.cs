using OnlineConsulting.Modules.Memberships.Domain;

namespace OnlineConsulting.Modules.Memberships.Tests.CustomerMemberships;

public class CustomerMembershipTests
{
    private static readonly DateTimeOffset Now = new(2026, 10, 3, 12, 0, 0, TimeSpan.Zero);
    private static readonly Guid PlanId = Guid.NewGuid();

    private static CustomerMembership Signup() => CustomerMembership.Start(Guid.NewGuid(), PlanId, Now);

    private static CustomerMembership Subscribed()
    {
        var membership = Signup();
        membership.AttachSubscription("sub_1", Now.AddMonths(1), null, null);
        return membership;
    }

    private static CustomerMembership ActiveMembership()
    {
        var membership = Subscribed();
        membership.Activate();
        return membership;
    }

    private static CustomerMembership CancelledMembership()
    {
        var membership = ActiveMembership();
        membership.Cancel();
        return membership;
    }

    [Fact]
    public void Start_AwaitsFirstPayment()
    {
        var membership = Signup();

        Assert.Equal(CustomerMembershipStatuses.PendingPayment, membership.Status);
        Assert.True(membership.IsAwaitingFirstPayment);
        Assert.Equal(PlanId, membership.MembershipPlanId);
        Assert.Equal(Now, membership.StartDate);
        Assert.False(membership.CanBePaused);
    }

    [Fact]
    public void SwitchPendingPlan_WithoutSubscription_ChangesPlan()
    {
        var membership = Signup();
        var otherPlan = Guid.NewGuid();

        membership.SwitchPendingPlan(otherPlan);

        Assert.Equal(otherPlan, membership.MembershipPlanId);
    }

    [Fact]
    public void SwitchPendingPlan_WithSubscription_Throws() =>
        Assert.Throws<InvalidOperationException>(() => Subscribed().SwitchPendingPlan(Guid.NewGuid()));

    [Fact]
    public void AttachSubscription_RecordsSubscriptionDetails()
    {
        var membership = Signup();
        var promoId = Guid.NewGuid();

        membership.AttachSubscription("sub_1", Now.AddMonths(1), Now.AddDays(14), promoId);

        Assert.Equal("sub_1", membership.ProviderSubscriptionId);
        Assert.Equal(Now.AddMonths(1), membership.RenewalDate);
        Assert.Equal(Now.AddDays(14), membership.TrialEndDate);
        Assert.Equal(promoId, membership.PromoCodeId);
        Assert.True(membership.IsAwaitingFirstPayment);
    }

    [Fact]
    public void AttachSubscription_Twice_Throws() =>
        Assert.Throws<InvalidOperationException>(() => Subscribed().AttachSubscription("sub_2", null, null, null));

    [Fact]
    public void Activate_FromFailedSignup_Activates()
    {
        var membership = Subscribed();
        membership.MarkSignupFailed();

        membership.Activate();

        Assert.Equal(CustomerMembershipStatuses.Active, membership.Status);
    }

    [Fact]
    public void Activate_WhenAlreadyActive_Throws() =>
        Assert.Throws<InvalidOperationException>(ActiveMembership().Activate);

    [Fact]
    public void MarkPaymentFailed_KeepsFirstFailureTime()
    {
        var membership = ActiveMembership();

        membership.MarkPaymentFailed(Now);
        membership.MarkPaymentFailed(Now.AddDays(3));

        Assert.Equal(CustomerMembershipStatuses.PastDue, membership.Status);
        Assert.Equal(Now, membership.PastDueSince);
    }

    [Fact]
    public void Renew_FromPastDue_ReactivatesAndClearsPastDue()
    {
        var membership = ActiveMembership();
        membership.MarkPaymentFailed(Now);

        membership.Renew(Now.AddMonths(2));

        Assert.Equal(CustomerMembershipStatuses.Active, membership.Status);
        Assert.Null(membership.PastDueSince);
        Assert.Equal(Now.AddMonths(2), membership.RenewalDate);
    }

    public static TheoryData<Action<CustomerMembership>> ActionsNotAllowedWhenCancelled => new()
    {
        m => m.Renew(Now),
        m => m.MarkPaymentFailed(Now),
        m => m.MarkSignupFailed(),
        m => m.Cancel(),
        m => m.CancelAtEndOfPeriod(),
        m => m.Reactivate(),
        m => m.Pause(),
        m => m.Resume(),
        m => m.ChangePlan(Guid.NewGuid()),
    };

    [Theory]
    [MemberData(nameof(ActionsNotAllowedWhenCancelled))]
    public void Action_WhenCancelled_Throws(Action<CustomerMembership> action) =>
        Assert.Throws<InvalidOperationException>(() => action(CancelledMembership()));

    [Fact]
    public void Pause_ThenResume_ReturnsToActive()
    {
        var membership = ActiveMembership();

        membership.Pause();
        Assert.Equal(CustomerMembershipStatuses.Paused, membership.Status);
        Assert.True(membership.CanBeResumed);

        membership.Resume();
        Assert.Equal(CustomerMembershipStatuses.Active, membership.Status);
    }

    [Fact]
    public void Pause_WhenNotActive_Throws() =>
        Assert.Throws<InvalidOperationException>(Signup().Pause);

    [Fact]
    public void Resume_WhenNotPaused_Throws() =>
        Assert.Throws<InvalidOperationException>(ActiveMembership().Resume);

    [Fact]
    public void ChangePlan_WhenActive_SwitchesPlan()
    {
        var membership = ActiveMembership();
        var otherPlan = Guid.NewGuid();

        membership.ChangePlan(otherPlan);

        Assert.Equal(otherPlan, membership.MembershipPlanId);
    }

    [Fact]
    public void ChangePlan_ToSamePlan_Throws() =>
        Assert.Throws<InvalidOperationException>(() => ActiveMembership().ChangePlan(PlanId));

    [Fact]
    public void ChangePlan_WhenEnding_Throws()
    {
        var membership = ActiveMembership();
        membership.CancelAtEndOfPeriod();

        _ = Assert.Throws<InvalidOperationException>(() => membership.ChangePlan(Guid.NewGuid()));
    }

    [Fact]
    public void CancelAtEndOfPeriod_KeepsStatusAndCanBeUndone()
    {
        var membership = ActiveMembership();

        membership.CancelAtEndOfPeriod();
        Assert.True(membership.CancelAtPeriodEnd);
        Assert.Equal(CustomerMembershipStatuses.Active, membership.Status);
        Assert.True(membership.CanBeReactivated);
        Assert.False(membership.CanBeCancelledAtPeriodEnd);

        membership.Reactivate();
        Assert.False(membership.CancelAtPeriodEnd);
    }

    [Fact]
    public void CancelAtEndOfPeriod_Twice_Throws()
    {
        var membership = ActiveMembership();
        membership.CancelAtEndOfPeriod();

        _ = Assert.Throws<InvalidOperationException>(membership.CancelAtEndOfPeriod);
    }

    [Fact]
    public void Reactivate_WhenNotEnding_Throws() =>
        Assert.Throws<InvalidOperationException>(ActiveMembership().Reactivate);

    [Fact]
    public void Cancel_EndsNowAndClearsPastDue()
    {
        var membership = ActiveMembership();
        membership.MarkPaymentFailed(Now);

        membership.Cancel();

        Assert.True(membership.IsCancelled);
        Assert.Null(membership.PastDueSince);
    }
}
