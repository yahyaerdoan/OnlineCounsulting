using Core.ApplicationLayer.Pipelines.Transactions.Extensions;
using Core.PersistenceLayer.Repositories.Auditing;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnlineConsulting.Modules.FeatureFlags.Application.Common;
using OnlineConsulting.Modules.FeatureFlags.Application.Features.FeatureFlags.Abstractions;
using OnlineConsulting.Modules.FeatureFlags.Application.Features.FeatureFlags;
using OnlineConsulting.Modules.FeatureFlags.Application;
using OnlineConsulting.Modules.FeatureFlags.Infrastructure.Caching;
using OnlineConsulting.Modules.FeatureFlags.Infrastructure.Persistence;
using OnlineConsulting.Modules.FeatureFlags.Infrastructure.Repositories;
using OnlineConsulting.Modules.FeatureFlags.Infrastructure.Writing;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.FeatureFlags;
using OnlineConsulting.SharedKernel.Tenancy;
using OnlineConsulting.SharedKernel.Transactions;

namespace OnlineConsulting.Modules.FeatureFlags.Infrastructure;

public static class FeatureFlagsModule
{
    public static IServiceCollection AddFeatureFlagsModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        _ = services.AddScoped<TenantSaveChangesInterceptor>();

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
        _ = services.AddTransactionalDbContext<IFeatureFlagsTransactionRequest, FeatureFlagsDbContext>();

        _ = services.AddSingleton<IDefaultAdminPermissions>(new DefaultAdminPermissions(FeatureFlagsOperationClaims.All));
        return services;
    }
}
