using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace OnlineConsulting.DataAccess.Concretions.Contexts;

public class DesingTimeDbContextFactory : IDesignTimeDbContextFactory<OnlineConsultingDbContext>
{
    OnlineConsultingDbContext IDesignTimeDbContextFactory<OnlineConsultingDbContext>.CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();
        var connectionString = configuration.GetSection("OnlineConsultingDbConnections:DevelopmentDbConnection").Value;
        var optionsBuilder = new DbContextOptionsBuilder<OnlineConsultingDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        IHttpContextAccessor httpContextAccessor = new DummyHttpContextAccessor();

        return new OnlineConsultingDbContext(optionsBuilder.Options, httpContextAccessor);
    }
}
public class DummyHttpContextAccessor : IHttpContextAccessor
{
    public HttpContext? HttpContext { get; set; } = null;
}
