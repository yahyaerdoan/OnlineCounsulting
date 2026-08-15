using Microsoft.EntityFrameworkCore.Design;
using OnlineConsulting.SharedKernel.Persistence;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.FeatureFlags.Infrastructure.Persistence;

/// <summary>Design-time only - the real ITenantProvider comes from the request scope at runtime.</summary>
public class FeatureFlagsDbContextFactory : IDesignTimeDbContextFactory<FeatureFlagsDbContext>
{
    public FeatureFlagsDbContext CreateDbContext(string[] args) => new(DesignTimeDbContextOptionsFactory.Build<FeatureFlagsDbContext>(), new NullTenantProvider());
}
