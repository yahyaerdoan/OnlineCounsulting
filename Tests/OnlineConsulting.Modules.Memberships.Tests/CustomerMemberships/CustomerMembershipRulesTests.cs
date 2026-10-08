using OnlineConsulting.Modules.Memberships.Domain;

namespace OnlineConsulting.Modules.Memberships.Tests.CustomerMemberships;

public class CustomerMembershipRulesTests
{
    [Theory]
    [InlineData(CustomerMembershipStatuses.PendingPayment, true)]
    [InlineData(CustomerMembershipStatuses.Failed, true)]
    [InlineData(CustomerMembershipStatuses.Active, false)]
    [InlineData(CustomerMembershipStatuses.PastDue, false)]
    public void IsAwaitingFirstPayment(string status, bool expected) =>
        Assert.Equal(expected, CustomerMembershipRules.IsAwaitingFirstPayment(status));

    [Theory]
    [InlineData(CustomerMembershipStatuses.Active, false, true)]
    [InlineData(CustomerMembershipStatuses.Active, true, false)]
    [InlineData(CustomerMembershipStatuses.Paused, false, false)]
    [InlineData(CustomerMembershipStatuses.PastDue, false, false)]
    public void CanChangePlan(string status, bool cancelAtPeriodEnd, bool expected) =>
        Assert.Equal(expected, CustomerMembershipRules.CanChangePlan(status, cancelAtPeriodEnd));

    [Theory]
    [InlineData(CustomerMembershipStatuses.Active, false, true)]
    [InlineData(CustomerMembershipStatuses.Paused, false, true)]
    [InlineData(CustomerMembershipStatuses.Active, true, false)]
    [InlineData(CustomerMembershipStatuses.Cancelled, false, false)]
    public void CanBeCancelledAtPeriodEnd(string status, bool cancelAtPeriodEnd, bool expected) =>
        Assert.Equal(expected, CustomerMembershipRules.CanBeCancelledAtPeriodEnd(status, cancelAtPeriodEnd));

    [Theory]
    [InlineData(CustomerMembershipStatuses.Active, true, true)]
    [InlineData(CustomerMembershipStatuses.Paused, true, true)]
    [InlineData(CustomerMembershipStatuses.Active, false, false)]
    [InlineData(CustomerMembershipStatuses.Cancelled, true, false)]
    public void CanBeReactivated(string status, bool cancelAtPeriodEnd, bool expected) =>
        Assert.Equal(expected, CustomerMembershipRules.CanBeReactivated(status, cancelAtPeriodEnd));

    [Theory]
    [InlineData(CustomerMembershipStatuses.Active, true)]
    [InlineData(CustomerMembershipStatuses.Paused, false)]
    public void CanBePaused(string status, bool expected) =>
        Assert.Equal(expected, CustomerMembershipRules.CanBePaused(status));

    [Theory]
    [InlineData(CustomerMembershipStatuses.Paused, true)]
    [InlineData(CustomerMembershipStatuses.Active, false)]
    public void CanBeResumed(string status, bool expected) =>
        Assert.Equal(expected, CustomerMembershipRules.CanBeResumed(status));
}
