using OnlineConsulting.Modules.Memberships.Domain;
using OnlineConsulting.SharedKernel.Payments;

namespace OnlineConsulting.Modules.Memberships.Tests.MembershipPlans;

public class MembershipPlanTests
{
    private static MembershipPlan Plan(int? trialDays = null) =>
        MembershipPlan.Create("Comfort Club", BillingCycles.Monthly, 19.99m, 2, 15m, 0m, "Priority booking", trialDays);

    [Fact]
    public void Create_IsActiveWithoutProviderPrice()
    {
        var plan = Plan(trialDays: 14);

        Assert.True(plan.IsActive);
        Assert.Equal(19.99m, plan.Price);
        Assert.Equal(14, plan.TrialDays);
        Assert.Null(plan.ProviderPriceId);
    }

    [Fact]
    public void Create_WithUnknownBillingCycle_Throws() =>
        Assert.Throws<ArgumentException>(() => MembershipPlan.Create("Plan", "Weekly", 10m, 0, 0m, 0m, null, null));

    [Theory]
    [InlineData(0, 0, 0, 0, null)]
    [InlineData(10, -1, 0, 0, null)]
    [InlineData(10, 0, 101, 0, null)]
    [InlineData(10, 0, -1, 0, null)]
    [InlineData(10, 0, 0, -1, null)]
    [InlineData(10, 0, 0, 0, 0)]
    public void Create_WithOutOfRangeValue_Throws(decimal price, int visits, decimal discount, decimal credit, int? trialDays) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => MembershipPlan.Create("Plan", BillingCycles.Annual, price, visits, discount, credit, null, trialDays));

    [Fact]
    public void UpdateDetails_KeepsPriceAndCycle()
    {
        var plan = Plan();

        plan.UpdateDetails("Comfort Club Plus", 4, 20m, 10m, "More visits");

        Assert.Equal("Comfort Club Plus", plan.Name);
        Assert.Equal(4, plan.IncludedVisitsPerYear);
        Assert.Equal(20m, plan.DiscountPercent);
        Assert.Equal(19.99m, plan.Price);
        Assert.Equal(BillingCycles.Monthly, plan.BillingCycle);
    }

    [Fact]
    public void UpdateDetails_WithDiscountOver100_Throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => Plan().UpdateDetails("Plan", 0, 150m, 0m, null));

    [Fact]
    public void AttachProviderPrice_OnlyOnce()
    {
        var plan = Plan();

        plan.AttachProviderPrice("prod_1", "price_1");

        Assert.Equal("price_1", plan.ProviderPriceId);
        _ = Assert.Throws<InvalidOperationException>(() => plan.AttachProviderPrice("prod_2", "price_2"));
    }

    [Fact]
    public void SetActive_ArchivesAndRestores()
    {
        var plan = Plan();

        plan.SetActive(false);
        Assert.False(plan.IsActive);

        plan.SetActive(true);
        Assert.True(plan.IsActive);
    }
}
