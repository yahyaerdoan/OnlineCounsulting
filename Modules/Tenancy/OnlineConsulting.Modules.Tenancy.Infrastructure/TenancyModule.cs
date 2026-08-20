using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnlineConsulting.Modules.Tenancy.Application;
using OnlineConsulting.Modules.Tenancy.Application.Features.Bundles.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.ModuleOfferings.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptionItems.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptions.Abstractions;
using OnlineConsulting.Modules.Tenancy.Infrastructure.Cleanup;
using OnlineConsulting.Modules.Tenancy.Infrastructure.Persistence;
using OnlineConsulting.Modules.Tenancy.Infrastructure.Pipelines;
using OnlineConsulting.Modules.Tenancy.Infrastructure.Pricing;
using OnlineConsulting.Modules.Tenancy.Infrastructure.Repositories;
using OnlineConsulting.Modules.Tenancy.Infrastructure.Status;
using OnlineConsulting.SharedKernel.Auditing;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Tenancy.Infrastructure;

public static class TenancyModule
{
    /// <summary>Tenant/ModuleOffering/Bundle/TenantSubscription/TenantSubscriptionItem are platform-owner data, not tenant-scoped - no TenantSaveChangesInterceptor is registered for this context.</summary>
    public static IServiceCollection AddTenancyModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        _ = services.AddScoped<AuditSaveChangesInterceptor>();

        _ = services.AddDbContext<TenancyDbContext>((serviceProvider, options) => options.UseSqlServer(connectionString)
            .AddInterceptors(serviceProvider.GetRequiredService<AuditSaveChangesInterceptor>()));

        _ = services.AddScoped<ITenantRepository, TenantRepository>();
        _ = services.AddScoped<IModuleOfferingRepository, ModuleOfferingRepository>();
        _ = services.AddScoped<IBundleRepository, BundleRepository>();
        _ = services.AddScoped<ITenantSubscriptionRepository, TenantSubscriptionRepository>();
        _ = services.AddScoped<ITenantSubscriptionItemRepository, TenantSubscriptionItemRepository>();
        _ = services.AddScoped<ITenantStatusReader, TenantStatusReader>();
        _ = services.AddScoped<ITenantModulePricingReader, TenantModulePricingReader>();
        _ = services.AddScoped<ITenantOwnershipReader, TenantOwnershipReader>();

        _ = services.Configure<TenancyCleanupOptions>(configuration.GetSection("Tenancy:OrphanCleanup"));
        _ = services.AddHostedService<OrphanedTenantCleanupService>();

        _ = services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AssemblyMarker).Assembly));
        _ = services.AddValidatorsFromAssembly(typeof(AssemblyMarker).Assembly);
        _ = services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TenancyTransactionAddingBehavior<,>));

        return services;
    }
}
