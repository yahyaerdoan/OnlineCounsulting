using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace OnlineConsulting.SharedKernel.Persistence;

/// <summary>Shared boilerplate for `IDesignTimeDbContextFactory` implementations - `dotnet ef` runs outside DI, so options must be built by hand.</summary>
public static class DesignTimeDbContextOptionsFactory
{
    public static DbContextOptions<TContext> Build<TContext>() where TContext : DbContext
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        return new DbContextOptionsBuilder<TContext>().UseSqlServer(connectionString).Options;
    }
}
