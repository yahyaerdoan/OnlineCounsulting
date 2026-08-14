using Microsoft.EntityFrameworkCore.Design;
using OnlineConsulting.SharedKernel.Persistence;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Categories.Infrastructure.Persistence;

/// <summary>Design-time only - the real ITenantProvider comes from the request scope at runtime.</summary>
public class CategoriesDbContextFactory : IDesignTimeDbContextFactory<CategoriesDbContext>
{
    public CategoriesDbContext CreateDbContext(string[] args) => new(DesignTimeDbContextOptionsFactory.Build<CategoriesDbContext>(), new NullTenantProvider());
}
