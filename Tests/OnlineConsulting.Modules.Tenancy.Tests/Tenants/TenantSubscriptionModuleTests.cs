using OnlineConsulting.Modules.Tenancy.Domain;

namespace OnlineConsulting.Modules.Tenancy.Tests.Tenants;

public class TenantSubscriptionModuleTests
{
    private static readonly DateTimeOffset Now = new(2026, 10, 3, 12, 0, 0, TimeSpan.Zero);

    private static TenantSubscription WithModules(params string[] activeKeys)
    {
        var subscription = TenantSubscription.Start(Guid.NewGuid(), Now.UtcDateTime);
        foreach (var key in activeKeys)
        {
            _ = subscription.AddModule(key, 49m, Now);
            subscription.ActivateModule(key, $"si_{key}");
        }

        return subscription;
    }

    [Fact]
    public void AddModule_AwaitsBillingWithPrice()
    {
        var subscription = WithModules();

        var item = subscription.AddModule("Scheduling", 49m, Now);

        Assert.Equal(TenantSubscriptionItemStatuses.Pending, item.Status);
        Assert.True(item.IsAwaitingBilling);
        Assert.Equal(49m, item.PriceAtAddition);
        Assert.Equal(Now.UtcDateTime, item.AddedAt);
        Assert.Same(item, Assert.Single(subscription.Items));
    }

    [Fact]
    public void AddModule_WithNegativePrice_Throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => WithModules().AddModule("Scheduling", -1m, Now));

    [Fact]
    public void AddModule_AfterFailedBilling_RetriesTheSameItem()
    {
        var subscription = WithModules();
        var first = subscription.AddModule("Scheduling", 49m, Now);
        subscription.FailModuleBilling("Scheduling");

        var retry = subscription.AddModule("Scheduling", 49m, Now);

        Assert.Same(first, retry);
        Assert.Single(subscription.Items);
    }

    [Fact]
    public void AddModule_WhenActive_Throws() =>
        Assert.Throws<InvalidOperationException>(() => WithModules("Scheduling").AddModule("Scheduling", 49m, Now));

    [Theory]
    [InlineData("si_1")]
    [InlineData(null)]
    public void ActivateModule_RecordsProviderItem(string? providerItemId)
    {
        var subscription = WithModules();
        var item = subscription.AddModule("Scheduling", 49m, Now);

        subscription.ActivateModule("Scheduling", providerItemId);

        Assert.Equal(TenantSubscriptionItemStatuses.Active, item.Status);
        Assert.Equal(providerItemId, item.ProviderSubscriptionItemId);
        Assert.True(subscription.HasActiveModule("Scheduling"));
    }

    [Fact]
    public void ActivateModule_WhenActive_Throws()
    {
        var subscription = WithModules("Scheduling");
        _ = Assert.Throws<InvalidOperationException>(() => subscription.ActivateModule("Scheduling", "si_2"));
        _ = Assert.Throws<InvalidOperationException>(() => subscription.FailModuleBilling("Scheduling"));
    }

    [Fact]
    public void ScheduleModuleRemoval_KeepsModuleUntilRenewalAndStopsRenewing()
    {
        var subscription = WithModules("Scheduling", "Memberships");
        var renewal = Now.UtcDateTime.AddDays(20);
        subscription.AttachProviderSubscription("sub_1", renewal, paid: true);
        var item = subscription.ActiveItems.Single(i => i.ModuleKey == "Scheduling");

        var endsAt = subscription.ScheduleModuleRemoval("Scheduling", Now.UtcDateTime);

        Assert.Equal(renewal, endsAt);
        Assert.Equal(renewal, item.EndsAt);
        Assert.Null(item.ProviderSubscriptionItemId);
        Assert.True(subscription.HasActiveModule("Scheduling"));
        Assert.True(subscription.IsModuleEnding("Scheduling"));
        Assert.DoesNotContain(item, subscription.RenewingItems);
    }

    [Fact]
    public void ResumeModule_RenewsAgain()
    {
        var subscription = WithModules("Scheduling", "Memberships");
        _ = subscription.ScheduleModuleRemoval("Scheduling", Now.UtcDateTime);

        subscription.ResumeModule("Scheduling", "si_new");

        var item = subscription.ActiveItems.Single(i => i.ModuleKey == "Scheduling");
        Assert.Null(item.EndsAt);
        Assert.Equal("si_new", item.ProviderSubscriptionItemId);
        Assert.False(subscription.IsModuleEnding("Scheduling"));
    }

    [Fact]
    public void EndDueModules_RemovesOnlyEndedModulesAndAllowsAddingAgain()
    {
        var subscription = WithModules("Scheduling", "Memberships", "Inquiries");
        subscription.AttachProviderSubscription("sub_1", Now.UtcDateTime.AddDays(20), paid: true);
        _ = subscription.ScheduleModuleRemoval("Scheduling", Now.UtcDateTime);
        var item = subscription.ActiveItems.Single(i => i.ModuleKey == "Scheduling");

        Assert.Empty(subscription.EndDueModules(Now.AddDays(19)));

        var ended = subscription.EndDueModules(Now.AddDays(20));

        Assert.Equal(["Scheduling"], ended);
        Assert.NotNull(item.DeletedDate);
        Assert.False(subscription.HasActiveModule("Scheduling"));

        var readded = subscription.AddModule("Scheduling", 59m, Now);
        Assert.NotSame(item, readded);
        Assert.Equal(59m, readded.PriceAtAddition);
    }

    [Fact]
    public void ScheduleModuleRemoval_LastRenewingOrInactiveModule_Throws()
    {
        var subscription = WithModules("Scheduling", "Memberships");
        _ = subscription.ScheduleModuleRemoval("Memberships", Now.UtcDateTime);

        Assert.False(subscription.CanRemoveModule("Scheduling"));
        Assert.False(subscription.CanRemoveModule("Memberships"));
        Assert.False(subscription.CanRemoveModule("Inquiries"));
        _ = Assert.Throws<InvalidOperationException>(() => subscription.ScheduleModuleRemoval("Scheduling", Now.UtcDateTime));
    }

    [Fact]
    public void SelectSignupModules_BeforeProviderSubscription_AddsMissingAndDropsUnchosen()
    {
        var subscription = WithModules();
        subscription.SelectSignupModules(new Dictionary<string, decimal> { ["Scheduling"] = 49m, ["Memberships"] = 29m }, Now);

        subscription.SelectSignupModules(new Dictionary<string, decimal> { ["Scheduling"] = 49m, ["Referrals"] = 19m }, Now);

        Assert.Equal(["Scheduling", "Referrals"], subscription.Items.Select(i => i.ModuleKey));
    }

    [Fact]
    public void SelectSignupModules_AfterProviderSubscription_OnlyAddsMissing()
    {
        var subscription = WithModules();
        subscription.SelectSignupModules(new Dictionary<string, decimal> { ["Scheduling"] = 49m }, Now);
        subscription.AttachProviderSubscription("sub_1", Now.UtcDateTime.AddMonths(1), paid: true);

        subscription.SelectSignupModules(new Dictionary<string, decimal> { ["Referrals"] = 19m }, Now);

        Assert.Equal(["Scheduling", "Referrals"], subscription.Items.Select(i => i.ModuleKey));
    }

    [Fact]
    public void CancelSignup_PutsModulesBackToAwaitingBilling()
    {
        var subscription = WithModules("Scheduling");

        subscription.CancelSignup();

        var item = Assert.Single(subscription.Items);
        Assert.Equal(TenantSubscriptionItemStatuses.Pending, item.Status);
        Assert.Null(item.ProviderSubscriptionItemId);
        Assert.True(subscription.IsCancelled);
    }
}
