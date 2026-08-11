using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OnlineConsulting.DataAccess.Concretions.Configurations.AppSettingConfigurations.AppSettingOptions;
using OnlineConsulting.DataAccess.Concretions.Contexts;

namespace OnlineConsulting.DataAccess.Concretions.Configurations.Extensions;

public static class ServiceRegistration
{
    public static void AddDataAccesssServiceRegistration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<AppSettingOption>().Bind(configuration.GetSection(AppSettingOption.DbConnection));

        services.AddDbContextPool<OnlineConsultingDbContext>((serviceProvider, options) =>
        {
            var appSettings = serviceProvider.GetRequiredService<IOptions<AppSettingOption>>().Value;
            options.UseSqlServer(appSettings.DevelopmentDbConnection, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(5), errorNumbersToAdd: null);
            });
        });
    }
}
