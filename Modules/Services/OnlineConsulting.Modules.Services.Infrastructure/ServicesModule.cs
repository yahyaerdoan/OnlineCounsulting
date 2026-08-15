using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnlineConsulting.Modules.Services.Application;
using OnlineConsulting.Modules.Services.Infrastructure.Persistence;
using OnlineConsulting.Modules.Services.Infrastructure.Pipelines;
using OnlineConsulting.Modules.Services.Infrastructure.Repositories;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Services.Infrastructure;

public static class ServicesModule
{
    public static IServiceCollection AddServicesModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetSection("OnlineConsultingDbConnections:DevelopmentDbConnection").Value;

        services.AddScoped<TenantSaveChangesInterceptor>();

        services.AddDbContext<ServicesDbContext>((serviceProvider, options) => options.UseSqlServer(connectionString)
            .AddInterceptors(serviceProvider.GetRequiredService<TenantSaveChangesInterceptor>()));

        services.AddScoped<IServiceRepository, ServiceRepository>();
        services.AddScoped<IServiceMediaItemRepository, ServiceMediaItemRepository>();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AssemblyMarker).Assembly));
        services.AddValidatorsFromAssembly(typeof(AssemblyMarker).Assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ServicesTransactionAddingBehavior<,>));

        return services;
    }
}
