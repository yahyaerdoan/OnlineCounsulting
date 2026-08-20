using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnlineConsulting.Modules.FeatureFlags.Application;
using OnlineConsulting.Modules.FeatureFlags.Application.Abstractions;
using OnlineConsulting.Modules.FeatureFlags.Application.Features.SetFeatureFlag;
using OnlineConsulting.Modules.FeatureFlags.Infrastructure.Caching;
using OnlineConsulting.Modules.FeatureFlags.Infrastructure.Persistence;
using OnlineConsulting.Modules.FeatureFlags.Infrastructure.Pipelines;
using OnlineConsulting.Modules.FeatureFlags.Infrastructure.Repositories;
using OnlineConsulting.Modules.FeatureFlags.Infrastructure.Writing;
using OnlineConsulting.SharedKernel.Auditing;
using OnlineConsulting.SharedKernel.FeatureFlags;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.FeatureFlags.Infrastructure;

public static class FeatureFlagsModule
{
    public static IServiceCollection AddFeatureFlagsModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        _ = services.AddScoped<TenantSaveChangesInterceptor>();
        _ = services.AddScoped<AuditSaveChangesInterceptor>();

        _ = services.AddDbContext<FeatureFlagsDbContext>((serviceProvider, options) => options.UseSqlServer(connectionString)
            .AddInterceptors(serviceProvider.GetRequiredService<TenantSaveChangesInterceptor>(), serviceProvider.GetRequiredService<AuditSaveChangesInterceptor>()));

        _ = services.AddMemoryCache();

        _ = services.AddScoped<IFeatureFlagRepository, FeatureFlagRepository>();
        _ = services.AddScoped<FeatureFlagCache>();
        _ = services.AddScoped<IFeatureFlagReader>(sp => sp.GetRequiredService<FeatureFlagCache>());
        _ = services.AddScoped<IFeatureFlagCacheInvalidator>(sp => sp.GetRequiredService<FeatureFlagCache>());
        _ = services.AddScoped<FeatureFlagUpserter>();
        _ = services.AddScoped<IFeatureFlagWriter, FeatureFlagWriter>();

        _ = services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AssemblyMarker).Assembly));
        _ = services.AddValidatorsFromAssembly(typeof(AssemblyMarker).Assembly);
        _ = services.AddTransient(typeof(IPipelineBehavior<,>), typeof(FeatureFlagsTransactionAddingBehavior<,>));

        return services;
    }
}
