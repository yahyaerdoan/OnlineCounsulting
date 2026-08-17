using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnlineConsulting.Modules.FeatureFlags.Application;
using OnlineConsulting.Modules.FeatureFlags.Application.Contracts;
using OnlineConsulting.Modules.FeatureFlags.Application.Abstractions;
using OnlineConsulting.Modules.FeatureFlags.Infrastructure.Caching;
using OnlineConsulting.Modules.FeatureFlags.Infrastructure.Persistence;
using OnlineConsulting.Modules.FeatureFlags.Infrastructure.Pipelines;
using OnlineConsulting.Modules.FeatureFlags.Infrastructure.Repositories;
using OnlineConsulting.SharedKernel.FeatureFlags;
using OnlineConsulting.SharedKernel.Auditing;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.FeatureFlags.Infrastructure;

public static class FeatureFlagsModule
{
    public static IServiceCollection AddFeatureFlagsModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetSection("OnlineConsultingDbConnections:DevelopmentDbConnection").Value;

        services.AddScoped<TenantSaveChangesInterceptor>();
        services.AddScoped<AuditSaveChangesInterceptor>();

        services.AddDbContext<FeatureFlagsDbContext>((serviceProvider, options) => options.UseSqlServer(connectionString)
            .AddInterceptors(serviceProvider.GetRequiredService<TenantSaveChangesInterceptor>(), serviceProvider.GetRequiredService<AuditSaveChangesInterceptor>()));

        services.AddMemoryCache();

        services.AddScoped<IFeatureFlagRepository, FeatureFlagRepository>();
        services.AddScoped<FeatureFlagCache>();
        services.AddScoped<IFeatureFlagReader>(sp => sp.GetRequiredService<FeatureFlagCache>());
        services.AddScoped<IFeatureFlagCacheInvalidator>(sp => sp.GetRequiredService<FeatureFlagCache>());

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AssemblyMarker).Assembly));
        services.AddValidatorsFromAssembly(typeof(AssemblyMarker).Assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(FeatureFlagsTransactionAddingBehavior<,>));

        return services;
    }
}
