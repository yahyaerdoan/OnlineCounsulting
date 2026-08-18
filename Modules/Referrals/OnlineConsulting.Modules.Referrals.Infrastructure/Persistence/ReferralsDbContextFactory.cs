using Microsoft.EntityFrameworkCore.Design;
using OnlineConsulting.SharedKernel.Persistence;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Referrals.Infrastructure.Persistence;

/// <summary>Design-time only - the real ITenantProvider comes from the request scope at runtime.</summary>
public class ReferralsDbContextFactory : IDesignTimeDbContextFactory<ReferralsDbContext>
{
    public ReferralsDbContext CreateDbContext(string[] args) => new(DesignTimeDbContextOptionsFactory.Build<ReferralsDbContext>(), new NullTenantProvider());
}
