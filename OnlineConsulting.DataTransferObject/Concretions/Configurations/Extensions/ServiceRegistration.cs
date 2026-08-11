using AutoMapper.Extensions.ExpressionMapping;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace OnlineConsulting.DataTransferObject.Concretions.Configurations.Extensions;

public static class ServiceRegistration
{
    public static void AddDataTransferObjectServiceRegistration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(cfg =>
        {
            cfg.AddMaps(Assembly.GetExecutingAssembly());
            cfg.AddExpressionMapping();
        });
    }
}
