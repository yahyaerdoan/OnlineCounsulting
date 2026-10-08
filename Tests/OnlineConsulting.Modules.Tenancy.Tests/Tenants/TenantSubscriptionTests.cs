using OnlineConsulting.Modules.Tenancy.Domain;

namespace OnlineConsulting.Modules.Tenancy.Tests.Tenants;

public class TenantSubscriptionTests
{
    private static readonly DateTime Now = new(2026, 10, 3, 12, 0, 0, DateTimeKind.Utc);

    private static TenantSubscription Started() => TenantSubscription.Start(Guid.NewGuid(), Now);

    private static TenantSubscription Attached(bool paid)
    {
        var subscription = Started();
        subscription.AttachProviderSubscription("sub_1", Now.AddMonths(1), paid);
        return subscription;
    }

    [Fact]
    public void Start_AwaitsPayment()
    {
        var subscription = Started();

        Assert.Equal(TenantSubscriptionStatuses.PendingPayment, subscription.Status);
        Assert.Null(subscription.ProviderSubscriptionId);
    }

    [Theory]
    [InlineData(true, TenantSubscriptionStatuses.Active)]
    [InlineData(false, TenantSubscriptionStatuses.PendingPayment)]
    public void AttachProviderSubscription_SetsStatusFromPayment(bool paid, string expected)
    {
        var subscription = Attached(paid);

        Assert.Equal(expected, subscription.Status);
        Assert.Equal("sub_1", subscription.ProviderSubscriptionId);
        Assert.Equal(Now.AddMonths(1), subscription.RenewalDate);
    }

    [Fact]
    public void AttachProviderSubscription_Twice_Throws() =>
        Assert.Throws<InvalidOperationException>(() => Attached(true).AttachProviderSubscription("sub_2", Now, true));

    [Fact]
    public void RestartSignup_AfterProviderSubscription_Throws() =>
        Assert.Throws<InvalidOperationException>(Attached(false).RestartSignup);

    [Fact]
    public void RecoverFromFailure_WithProviderSubscription_Activates()
    {
        var subscription = Attached(false);
        subscription.MarkFailed();

        subscription.RecoverFromFailure();

        Assert.Equal(TenantSubscriptionStatuses.Active, subscription.Status);
    }

    [Fact]
    public void RecoverFromFailure_WithoutProviderSubscription_Throws()
    {
        var subscription = Started();
        subscription.MarkFailed();

        _ = Assert.Throws<InvalidOperationException>(subscription.RecoverFromFailure);
    }

    [Fact]
    public void CancelSignup_ForgetsProviderSubscription()
    {
        var subscription = Attached(true);

        subscription.CancelSignup();

        Assert.True(subscription.IsCancelled);
        Assert.Null(subscription.ProviderSubscriptionId);
    }

    [Fact]
    public void PastDueThenRenew_ReturnsToActive()
    {
        var subscription = Attached(true);

        subscription.MarkPastDue();
        Assert.Equal(TenantSubscriptionStatuses.PastDue, subscription.Status);

        subscription.Renew(Now.AddMonths(2));
        Assert.Equal(TenantSubscriptionStatuses.Active, subscription.Status);
        Assert.Equal(Now.AddMonths(2), subscription.RenewalDate);
    }

    public static TheoryData<Action<TenantSubscription>> ActionsNotAllowedWhenCancelled => new()
    {
        s => s.Cancel(),
        s => s.MarkPastDue(),
        s => s.MarkFailed(),
        s => s.Renew(Now),
    };

    [Theory]
    [MemberData(nameof(ActionsNotAllowedWhenCancelled))]
    public void Action_WhenCancelled_Throws(Action<TenantSubscription> action)
    {
        var subscription = Attached(true);
        subscription.Cancel();

        _ = Assert.Throws<InvalidOperationException>(() => action(subscription));
    }
}
