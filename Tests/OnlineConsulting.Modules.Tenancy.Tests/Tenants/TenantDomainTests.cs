using OnlineConsulting.Modules.Tenancy.Domain;

namespace OnlineConsulting.Modules.Tenancy.Tests.Tenants;

public class TenantDomainTests
{
    [Fact]
    public void Register_normalizes_the_host_and_starts_pending()
    {
        var domain = TenantDomain.Register(Guid.NewGuid(), "  WWW.Example.COM. ", "domain_1");

        Assert.Equal("www.example.com", domain.Host);
        Assert.Equal(TenantDomainStatuses.Pending, domain.Status);
        Assert.False(domain.IsActive);
        Assert.Null(domain.ActivatedAt);
    }

    [Fact]
    public void Activate_marks_it_active_once()
    {
        var domain = TenantDomain.Register(Guid.NewGuid(), "www.example.com", "domain_1");
        var first = DateTimeOffset.UtcNow;

        domain.Activate(first);
        domain.Activate(first.AddMinutes(5));

        Assert.True(domain.IsActive);
        Assert.Equal(first, domain.ActivatedAt);
    }

    [Theory]
    [InlineData("", "domain_1")]
    [InlineData("www.example.com", " ")]
    public void Register_requires_host_and_provider_id(string host, string providerDomainId) =>
        Assert.ThrowsAny<ArgumentException>(() => TenantDomain.Register(Guid.NewGuid(), host, providerDomainId));
}
