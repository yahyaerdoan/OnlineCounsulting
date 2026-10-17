using Core.ApplicationLayer.Pipelines.Transactions.Extensions;
using Core.PersistenceLayer.Repositories.Auditing;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OnlineConsulting.Modules.Tenancy.Application.Features.Signup;
using OnlineConsulting.Modules.Tenancy.Application;
using OnlineConsulting.Modules.Tenancy.Application.Features.Bundles.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantDomains;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantDomains.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.ModuleOfferings.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptions.Abstractions;
using OnlineConsulting.Modules.Tenancy.Infrastructure.Branding;
using OnlineConsulting.Modules.Tenancy.Infrastructure.Cleanup;
using OnlineConsulting.Modules.Tenancy.Infrastructure.Contacts;
using OnlineConsulting.Modules.Tenancy.Infrastructure.Domains;
using OnlineConsulting.Modules.Tenancy.Infrastructure.Hosting;
using OnlineConsulting.Modules.Tenancy.Infrastructure.Notifications;
using OnlineConsulting.Modules.Tenancy.Infrastructure.Persistence;
using OnlineConsulting.Modules.Tenancy.Infrastructure.Pricing;
using OnlineConsulting.Modules.Tenancy.Infrastructure.Repositories;
using OnlineConsulting.Modules.Tenancy.Infrastructure.Status;
using OnlineConsulting.Modules.Tenancy.Infrastructure.StripeConnect;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants;
using OnlineConsulting.Modules.Tenancy.Infrastructure.TimeZones;
using OnlineConsulting.SharedKernel.Notifications;
using OnlineConsulting.SharedKernel.Tenancy;
using OnlineConsulting.SharedKernel.Transactions;

namespace OnlineConsulting.Modules.Tenancy.Infrastructure;

public static class TenancyModule
{
    /// <summary>Tenant/ModuleOffering/Bundle/TenantSubscription/TenantSubscriptionItem are platform-owner data, not tenant-scoped, so this context deliberately isn't a TenantDbContext.</summary>
    public static IServiceCollection AddTenancyModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        _ = services.AddDbContext<TenancyDbContext>((serviceProvider, options) => options.UseSqlServer(connectionString)
            .AddInterceptors(serviceProvider.GetRequiredService<AuditSaveChangesInterceptor>()));

        _ = services.AddScoped<ITenantRepository, TenantRepository>();
        _ = services.AddScoped<IModuleOfferingRepository, ModuleOfferingRepository>();
        _ = services.AddScoped<IBundleRepository, BundleRepository>();
        _ = services.AddScoped<ITenantSubscriptionRepository, TenantSubscriptionRepository>();
        _ = services.AddScoped<ITenantStatusReader, TenantStatusReader>();
        _ = services.AddMemoryCache();
        _ = services.AddScoped<TenantTimeZoneReader>();
        _ = services.AddScoped<ITenantTimeZoneReader>(sp => sp.GetRequiredService<TenantTimeZoneReader>());
        _ = services.AddScoped<ITenantTimeZoneCacheInvalidator>(sp => sp.GetRequiredService<TenantTimeZoneReader>());
        _ = services.AddScoped<ITenantModulePricingReader, TenantModulePricingReader>();
        _ = services.AddScoped<ITenantDomainRepository, TenantDomainRepository>();
        _ = services.AddScoped<ITenantHostCacheInvalidator, TenantHostCache>();
        _ = services.AddScoped<TenantDomainActivator>();
        _ = services.AddHttpClient(nameof(RailwayCustomDomainGateway));
        _ = services.AddKeyedSingleton<ICustomDomainGateway, MockCustomDomainGateway>(CustomDomainProviderNames.Mock);
        _ = services.AddKeyedSingleton<ICustomDomainGateway, RailwayCustomDomainGateway>(CustomDomainProviderNames.Railway);
        _ = services.AddHttpClient(nameof(CloudflareCustomDomainGateway));
        _ = services.AddKeyedSingleton<ICustomDomainGateway, CloudflareCustomDomainGateway>(CustomDomainProviderNames.Cloudflare);
        _ = services.AddSingleton(serviceProvider => serviceProvider.GetRequiredKeyedService<ICustomDomainGateway>(
            serviceProvider.GetRequiredService<IOptions<TenantDomainOptions>>().Value.Provider));
        _ = services.AddScoped<ITenantHostResolver, TenantHostResolver>();
        _ = services.AddScoped<ITenantOriginReader, TenantOriginReader>();
        _ = services.AddScoped<IApiOriginReader, ApiOriginReader>();
        _ = services.AddScoped<TenantBrandReader>();
        _ = services.AddScoped<ITenantBrandReader>(sp => sp.GetRequiredService<TenantBrandReader>());
        _ = services.AddScoped<ITenantBrandCacheInvalidator>(sp => sp.GetRequiredService<TenantBrandReader>());
        _ = services.AddScoped<ITenantContactReader, TenantContactReader>();
        _ = services.AddScoped<ITenantOwnershipReader, TenantOwnershipReader>();
        _ = services.AddScoped<ITenantPaymentAccountReader, TenantPaymentAccountReader>();
        _ = services.AddScoped<IStripeConnectStateProtector, StripeConnectStateProtector>();
        _ = services.AddScoped<StripeConnectStatusUpdater>();
        _ = services.AddScoped<StripeConnectOAuthCallbackHandler>();
        _ = services.AddScoped<TenantSubscriptionActivator>();
        _ = services.AddScoped<IEmailOutboxWriter<ITenancyOutboxModule>, EmailOutboxWriter>();
        _ = services.AddScoped<OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptions.TenantReceiptSender>();

        _ = services.Configure<TenancyCleanupOptions>(configuration.GetSection("Tenancy:OrphanCleanup"));
        _ = services.Configure<TenantTimeZoneOptions>(configuration.GetSection("Tenancy:TimeZone"));
        _ = services.Configure<TenantHostingOptions>(configuration.GetSection("Tenancy:Hosting"));
        _ = services.Configure<TenantBrandingOptions>(configuration.GetSection("Tenancy:Branding"));
        _ = services.Configure<TenantDomainOptions>(configuration.GetSection("Tenancy:CustomDomains"));
        _ = services.AddHostedService<OrphanedTenantCleanupService>();
        _ = services.AddHostedService<EndingModuleService>();
        _ = services.AddHostedService<TenantDomainActivationService>();

        _ = services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AssemblyMarker).Assembly));
        _ = services.AddValidatorsFromAssembly(typeof(AssemblyMarker).Assembly);
        _ = services.AddTransactionalDbContext<ITenancyTransactionRequest, TenancyDbContext>();

        return services;
    }
}
