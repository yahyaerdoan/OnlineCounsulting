using OnlineConsulting.Modules.Tenancy.Domain;

namespace OnlineConsulting.Modules.Tenancy.Tests.Tenants;

public class TenantSubscriptionItemTests
{
    private static readonly DateTime Now = new(2026, 10, 3, 12, 0, 0, DateTimeKind.Utc);

    private static TenantSubscriptionItem Added() => TenantSubscriptionItem.Add(Guid.NewGuid(), "Scheduling", 49m, Now);

    [Fact]
    public void Add_AwaitsBillingWithPrice()
    {
        var item = Added();

        Assert.Equal(TenantSubscriptionItemStatuses.Pending, item.Status);
        Assert.True(item.IsAwaitingBilling);
        Assert.Equal(49m, item.PriceAtAddition);
        Assert.Equal(Now, item.AddedAt);
    }

    [Fact]
    public void Add_WithNegativePrice_Throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => TenantSubscriptionItem.Add(Guid.NewGuid(), "Scheduling", -1m, Now));

    [Theory]
    [InlineData("si_1")]
    [InlineData(null)]
    public void Activate_RecordsProviderItem(string? providerItemId)
    {
        var item = Added();

        item.Activate(providerItemId);

        Assert.Equal(TenantSubscriptionItemStatuses.Active, item.Status);
        Assert.Equal(providerItemId, item.ProviderSubscriptionItemId);
    }

    [Fact]
    public void Activate_AfterFailedBilling_Activates()
    {
        var item = Added();
        item.MarkBillingFailed();

        item.Activate("si_1");

        Assert.Equal(TenantSubscriptionItemStatuses.Active, item.Status);
    }

    [Fact]
    public void Activate_WhenActive_Throws()
    {
        var item = Added();
        item.Activate("si_1");

        _ = Assert.Throws<InvalidOperationException>(() => item.Activate("si_2"));
        _ = Assert.Throws<InvalidOperationException>(item.MarkBillingFailed);
    }

    [Fact]
    public void ResetForRetry_ClearsBilling()
    {
        var item = Added();
        item.Activate("si_1");

        item.ResetForRetry();

        Assert.Equal(TenantSubscriptionItemStatuses.Pending, item.Status);
        Assert.Null(item.ProviderSubscriptionItemId);
    }
}
