using OnlineConsulting.Modules.Memberships.Domain;

namespace OnlineConsulting.Modules.Memberships.Tests.PromoCodes;

public class PromoCodeTests
{
    private static readonly DateTimeOffset Now = new(2026, 10, 3, 12, 0, 0, TimeSpan.Zero);
    private static readonly Guid PlanId = Guid.NewGuid();

    private static PromoCode Percent(decimal value = 20m, int? maxRedemptions = null, DateTimeOffset? expiresAt = null, Guid? planId = null) =>
        PromoCode.Create(" spring-20 ", PromoCodeDiscountTypes.Percent, value, maxRedemptions, expiresAt, planId);

    [Fact]
    public void Create_NormalizesCodeAndIsActive()
    {
        var promo = Percent();

        Assert.Equal("SPRING-20", promo.Code);
        Assert.True(promo.IsActive);
        Assert.Equal(0, promo.RedemptionCount);
    }

    [Fact]
    public void Create_WithUnknownType_Throws() =>
        Assert.Throws<ArgumentException>(() => PromoCode.Create("X", "Bogo", 10m, null, null, null));

    [Theory]
    [InlineData(PromoCodeDiscountTypes.Percent, 0)]
    [InlineData(PromoCodeDiscountTypes.Percent, 101)]
    [InlineData(PromoCodeDiscountTypes.Fixed, -5)]
    public void Create_WithOutOfRangeValue_Throws(string type, decimal value) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => PromoCode.Create("X", type, value, null, null, null));

    [Fact]
    public void Create_FixedOver100_IsAllowed() =>
        Assert.Equal(150m, PromoCode.Create("X", PromoCodeDiscountTypes.Fixed, 150m, null, null, null).DiscountValue);

    [Fact]
    public void CheckRedeemable_WhenUsable_IsNull() =>
        Assert.Null(Percent().CheckRedeemable(PlanId, Now));

    [Fact]
    public void CheckRedeemable_WhenInactive_Rejects()
    {
        var promo = Percent();
        promo.SetActive(false);

        Assert.Equal(PromoCodeRejection.Inactive, promo.CheckRedeemable(PlanId, Now));
    }

    [Fact]
    public void CheckRedeemable_WhenExpired_Rejects() =>
        Assert.Equal(PromoCodeRejection.Expired, Percent(expiresAt: Now.AddDays(-1)).CheckRedeemable(PlanId, Now));

    [Fact]
    public void CheckRedeemable_ForOtherPlan_Rejects() =>
        Assert.Equal(PromoCodeRejection.NotValidForPlan, Percent(planId: Guid.NewGuid()).CheckRedeemable(PlanId, Now));

    [Fact]
    public void Redeem_UpToTheLimit_ThenRejects()
    {
        var promo = Percent(maxRedemptions: 2);

        promo.Redeem(PlanId, Now);
        promo.Redeem(PlanId, Now);

        Assert.Equal(2, promo.RedemptionCount);
        Assert.Equal(PromoCodeRejection.RedemptionLimitReached, promo.CheckRedeemable(PlanId, Now));
        _ = Assert.Throws<InvalidOperationException>(() => promo.Redeem(PlanId, Now));
    }

    [Theory]
    [InlineData(PromoCodeDiscountTypes.Percent, 20, 19.99, 4.00)]
    [InlineData(PromoCodeDiscountTypes.Percent, 100, 19.99, 19.99)]
    [InlineData(PromoCodeDiscountTypes.Fixed, 5, 19.99, 5)]
    [InlineData(PromoCodeDiscountTypes.Fixed, 50, 19.99, 19.99)]
    public void DiscountFor_NeverExceedsThePrice(string type, decimal value, decimal price, decimal expected) =>
        Assert.Equal(expected, PromoCode.Create("X", type, value, null, null, null).DiscountFor(price));
}
