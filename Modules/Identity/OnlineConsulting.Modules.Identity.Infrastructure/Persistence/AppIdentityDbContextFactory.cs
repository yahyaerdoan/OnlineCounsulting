using Microsoft.EntityFrameworkCore.Design;
using OnlineConsulting.SharedKernel.Tenancy;
using OnlineConsulting.SharedKernel.Persistence;

namespace OnlineConsulting.Modules.Identity.Infrastructure.Persistence;

public class AppIdentityDbContextFactory : IDesignTimeDbContextFactory<AppIdentityDbContext>
{
    public AppIdentityDbContext CreateDbContext(string[] args) => new(DesignTimeDbContextOptionsFactory.Build<AppIdentityDbContext>(), new NullTenantProvider());
}
