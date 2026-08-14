using Microsoft.EntityFrameworkCore.Design;
using OnlineConsulting.SharedKernel.Persistence;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Commerce.Infrastructure.Persistence;

/// <summary>Design-time only - the real ITenantProvider comes from the request scope at runtime.</summary>
public class CommerceDbContextFactory : IDesignTimeDbContextFactory<CommerceDbContext>
{
    public CommerceDbContext CreateDbContext(string[] args) => new(DesignTimeDbContextOptionsFactory.Build<CommerceDbContext>(), new NullTenantProvider());
}
