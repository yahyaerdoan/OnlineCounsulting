using OnlineConsulting.Modules.Tenancy.Domain;

namespace OnlineConsulting.Modules.Tenancy.Tests.Tenants;

public class TenantRulesTests
{
    [Theory]
    [InlineData(TenantStatuses.Active, true)]
    [InlineData(TenantStatuses.PastDue, true)]
    [InlineData(TenantStatuses.Suspended, false)]
    [InlineData(TenantStatuses.PendingPayment, false)]
    public void CanBeSuspended(string status, bool expected) =>
        Assert.Equal(expected, TenantRules.CanBeSuspended(status));

    [Theory]
    [InlineData(TenantStatuses.Suspended, true)]
    [InlineData(TenantStatuses.Active, false)]
    [InlineData(TenantStatuses.Cancelled, false)]
    public void CanBeReactivated(string status, bool expected) =>
        Assert.Equal(expected, TenantRules.CanBeReactivated(status));

    [Theory]
    [InlineData(TenantStatuses.Cancelled, false)]
    [InlineData(TenantStatuses.Suspended, true)]
    [InlineData(TenantStatuses.Failed, true)]
    public void CanBeCancelled(string status, bool expected) =>
        Assert.Equal(expected, TenantRules.CanBeCancelled(status));

    [Theory]
    [InlineData(TenantStatuses.Suspended, true)]
    [InlineData(TenantStatuses.Cancelled, true)]
    [InlineData(TenantStatuses.PastDue, false)]
    [InlineData(TenantStatuses.Failed, false)]
    public void IsHeldByStaff(string status, bool expected) =>
        Assert.Equal(expected, TenantRules.IsHeldByStaff(status));

    [Theory]
    [InlineData(TenantStatuses.PendingPayment, true)]
    [InlineData(TenantStatuses.Failed, true)]
    [InlineData(TenantStatuses.Active, false)]
    public void IsAwaitingSignup(string status, bool expected) =>
        Assert.Equal(expected, TenantRules.IsAwaitingSignup(status));
}
