using Microsoft.EntityFrameworkCore.Design;
using OnlineConsulting.SharedKernel.Persistence;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Scheduling.Infrastructure.Persistence;

/// <summary>Design-time only - the real ITenantProvider comes from the request scope at runtime.</summary>
public class SchedulingDbContextFactory : IDesignTimeDbContextFactory<SchedulingDbContext>
{
    public SchedulingDbContext CreateDbContext(string[] args) => new(DesignTimeDbContextOptionsFactory.Build<SchedulingDbContext>(), new NullTenantProvider());
}
