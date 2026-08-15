using Microsoft.EntityFrameworkCore.Design;
using OnlineConsulting.SharedKernel.Persistence;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Media.Infrastructure.Persistence;

/// <summary>Design-time only - the real ITenantProvider comes from the request scope at runtime.</summary>
public class MediaDbContextFactory : IDesignTimeDbContextFactory<MediaDbContext>
{
    public MediaDbContext CreateDbContext(string[] args) => new(DesignTimeDbContextOptionsFactory.Build<MediaDbContext>(), new NullTenantProvider());
}
