using Microsoft.EntityFrameworkCore.Design;
using OnlineConsulting.SharedKernel.Persistence;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Memberships.Infrastructure.Persistence;

/// <summary>Design-time only - the real ITenantProvider comes from the request scope at runtime.</summary>
public class MembershipsDbContextFactory : IDesignTimeDbContextFactory<MembershipsDbContext>
{
    public MembershipsDbContext CreateDbContext(string[] args) => new(DesignTimeDbContextOptionsFactory.Build<MembershipsDbContext>(), new NullTenantProvider());
}
