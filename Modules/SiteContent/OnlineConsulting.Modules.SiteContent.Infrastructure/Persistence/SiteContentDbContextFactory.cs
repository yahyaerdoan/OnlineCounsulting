using Microsoft.EntityFrameworkCore.Design;
using OnlineConsulting.SharedKernel.Persistence;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.SiteContent.Infrastructure.Persistence;

/// <summary>Design-time only - the real ITenantProvider comes from the request scope at runtime.</summary>
public class SiteContentDbContextFactory : IDesignTimeDbContextFactory<SiteContentDbContext>
{
    public SiteContentDbContext CreateDbContext(string[] args) => new(DesignTimeDbContextOptionsFactory.Build<SiteContentDbContext>(), new NullTenantProvider());
}
