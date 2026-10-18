using OnlineConsulting.Modules.Tenancy.Domain;

namespace OnlineConsulting.Modules.Tenancy.Tests.Tenants;

public class TenantEmailDomainTests
{
    [Fact]
    public void Register_normalizes_the_domain_and_starts_pending()
    {
        var domain = TenantEmailDomain.Register(Guid.NewGuid(), "  Example.COM. ");

        Assert.Equal("example.com", domain.Domain);
        Assert.Equal("noreply@example.com", domain.SenderAddress);
        Assert.Equal(TenantEmailDomainStatuses.Pending, domain.Status);
        Assert.False(domain.IsActive);
    }

    [Fact]
    public void Activate_marks_it_active_once()
    {
        var domain = TenantEmailDomain.Register(Guid.NewGuid(), "example.com");
        var first = DateTimeOffset.UtcNow;

        domain.Activate(first);
        domain.Activate(first.AddMinutes(5));

        Assert.True(domain.IsActive);
        Assert.Equal(first, domain.ActivatedAt);
    }

    [Fact]
    public void Register_requires_a_domain() =>
        Assert.ThrowsAny<ArgumentException>(() => TenantEmailDomain.Register(Guid.NewGuid(), " "));
}
