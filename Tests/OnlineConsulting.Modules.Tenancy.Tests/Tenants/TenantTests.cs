using OnlineConsulting.Modules.Tenancy.Domain;

namespace OnlineConsulting.Modules.Tenancy.Tests.Tenants;

public class TenantTests
{
    private static Tenant Reserved() => Tenant.Reserve("Acme HVAC", "acme-hvac", "owner@acme.test");

    private static Tenant InStatus(string status)
    {
        var tenant = Reserved();
        switch (status)
        {
            case TenantStatuses.Active:
                tenant.CompleteSignup(TenantSubscriptionStatuses.Active);
                break;
            case TenantStatuses.PastDue:
                tenant.CompleteSignup(TenantSubscriptionStatuses.PastDue);
                break;
            case TenantStatuses.Failed:
                tenant.FailSignup();
                break;
            case TenantStatuses.Suspended:
                tenant.CompleteSignup(TenantSubscriptionStatuses.Active);
                tenant.Suspend();
                break;
            case TenantStatuses.Cancelled:
                tenant.Cancel();
                break;
        }

        return tenant;
    }

    [Fact]
    public void Reserve_AwaitsSignup()
    {
        var tenant = Reserved();

        Assert.Equal(TenantStatuses.PendingPayment, tenant.Status);
        Assert.True(tenant.IsAwaitingSignup);
        Assert.Equal("acme-hvac", tenant.Slug);
        Assert.Equal("owner@acme.test", tenant.PrimaryContactEmail);
    }

    [Theory]
    [InlineData("", "slug", "a@b.test")]
    [InlineData("Name", " ", "a@b.test")]
    [InlineData("Name", "slug", "")]
    public void Reserve_WithBlankValue_Throws(string name, string slug, string email) =>
        Assert.ThrowsAny<ArgumentException>(() => Tenant.Reserve(name, slug, email));

    [Theory]
    [InlineData(TenantSubscriptionStatuses.Active, TenantStatuses.Active)]
    [InlineData(TenantSubscriptionStatuses.PastDue, TenantStatuses.PastDue)]
    [InlineData(TenantSubscriptionStatuses.PendingPayment, TenantStatuses.PendingPayment)]
    public void CompleteSignup_FollowsSubscriptionStatus(string subscriptionStatus, string expected)
    {
        var tenant = Reserved();

        tenant.CompleteSignup(subscriptionStatus);

        Assert.Equal(expected, tenant.Status);
    }

    [Theory]
    [InlineData(TenantStatuses.Suspended)]
    [InlineData(TenantStatuses.Cancelled)]
    public void BillingEvents_WhenHeldByStaff_Throw(string status)
    {
        var tenant = InStatus(status);

        Assert.True(tenant.IsHeldByStaff);
        _ = Assert.Throws<InvalidOperationException>(() => tenant.CompleteSignup(TenantSubscriptionStatuses.Active));
        _ = Assert.Throws<InvalidOperationException>(tenant.ApplyPaymentFailed);
        _ = Assert.Throws<InvalidOperationException>(tenant.ApplyRenewal);
        _ = Assert.Throws<InvalidOperationException>(tenant.ApplySubscriptionEnded);
    }

    [Fact]
    public void BillingEvents_MoveAnActiveTenant()
    {
        var tenant = InStatus(TenantStatuses.Active);

        tenant.ApplyPaymentFailed();
        Assert.Equal(TenantStatuses.PastDue, tenant.Status);

        tenant.ApplyRenewal();
        Assert.Equal(TenantStatuses.Active, tenant.Status);

        tenant.ApplySubscriptionEnded();
        Assert.Equal(TenantStatuses.Suspended, tenant.Status);
    }

    [Theory]
    [InlineData(TenantStatuses.Active)]
    [InlineData(TenantStatuses.PastDue)]
    public void Suspend_ThenReactivate(string status)
    {
        var tenant = InStatus(status);

        tenant.Suspend();
        Assert.Equal(TenantStatuses.Suspended, tenant.Status);

        tenant.Reactivate();
        Assert.Equal(TenantStatuses.Active, tenant.Status);
    }

    [Theory]
    [InlineData(TenantStatuses.PendingPayment)]
    [InlineData(TenantStatuses.Failed)]
    [InlineData(TenantStatuses.Suspended)]
    [InlineData(TenantStatuses.Cancelled)]
    public void Suspend_WhenNotActiveOrPastDue_Throws(string status) =>
        Assert.Throws<InvalidOperationException>(InStatus(status).Suspend);

    [Fact]
    public void Reactivate_WhenNotSuspended_Throws() =>
        Assert.Throws<InvalidOperationException>(InStatus(TenantStatuses.Active).Reactivate);

    [Fact]
    public void Cancel_Twice_Throws() =>
        Assert.Throws<InvalidOperationException>(InStatus(TenantStatuses.Cancelled).Cancel);

    [Fact]
    public void FailSignup_WhenCancelled_Throws() =>
        Assert.Throws<InvalidOperationException>(InStatus(TenantStatuses.Cancelled).FailSignup);

    [Fact]
    public void LinkProviderCustomerAndAssignOwner_RecordValues()
    {
        var tenant = Reserved();
        var owner = Guid.NewGuid();

        tenant.LinkProviderCustomer("cus_1");
        tenant.AssignOwner(owner);

        Assert.Equal("cus_1", tenant.ProviderCustomerId);
        Assert.Equal(owner, tenant.OwnerUserId);
    }
}
