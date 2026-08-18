using Microsoft.EntityFrameworkCore.Design;
using OnlineConsulting.SharedKernel.Persistence;

namespace OnlineConsulting.Modules.Tenancy.Infrastructure.Persistence;

public class TenancyDbContextFactory : IDesignTimeDbContextFactory<TenancyDbContext>
{
    public TenancyDbContext CreateDbContext(string[] args) => new(DesignTimeDbContextOptionsFactory.Build<TenancyDbContext>());
}
