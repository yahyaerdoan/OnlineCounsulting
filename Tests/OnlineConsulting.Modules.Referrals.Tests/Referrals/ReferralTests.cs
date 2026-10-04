using OnlineConsulting.Modules.Referrals.Domain;

namespace OnlineConsulting.Modules.Referrals.Tests.Referrals;

public class ReferralTests
{
    private static readonly DateTimeOffset Now = new(2026, 10, 3, 12, 0, 0, TimeSpan.Zero);

    private static Referral Pending() => Referral.Create(Guid.NewGuid(), Guid.NewGuid(), "JANE-42");

    [Fact]
    public void Create_IsPending()
    {
        var referral = Pending();

        Assert.Equal(ReferralStatuses.Pending, referral.Status);
        Assert.False(referral.IsRewarded);
        Assert.Null(referral.RewardAmount);
    }

    [Fact]
    public void Create_ReferringYourself_Throws()
    {
        var user = Guid.NewGuid();

        _ = Assert.Throws<ArgumentException>(() => Referral.Create(user, user, "JANE-42"));
    }

    [Fact]
    public void Create_WithBlankCode_Throws() =>
        Assert.ThrowsAny<ArgumentException>(() => Referral.Create(Guid.NewGuid(), Guid.NewGuid(), " "));

    [Fact]
    public void Reward_RecordsAmountAndTime()
    {
        var referral = Pending();

        referral.Reward(25m, Now);

        Assert.True(referral.IsRewarded);
        Assert.Equal(25m, referral.RewardAmount);
        Assert.Equal(Now, referral.RewardedAt);
    }

    [Fact]
    public void Reward_Twice_Throws()
    {
        var referral = Pending();
        referral.Reward(25m, Now);

        _ = Assert.Throws<InvalidOperationException>(() => referral.Reward(25m, Now));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Reward_WithNonPositiveAmount_Throws(decimal amount) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => Pending().Reward(amount, Now));
}
