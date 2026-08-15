using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnlineConsulting.Modules.SiteContent.Application;
using OnlineConsulting.Modules.SiteContent.Infrastructure.Persistence;
using OnlineConsulting.Modules.SiteContent.Infrastructure.Pipelines;
using OnlineConsulting.Modules.SiteContent.Infrastructure.Repositories;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.SiteContent.Infrastructure;

public static class SiteContentModule
{
    public static IServiceCollection AddSiteContentModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetSection("OnlineConsultingDbConnections:DevelopmentDbConnection").Value;

        services.AddScoped<TenantSaveChangesInterceptor>();

        services.AddDbContext<SiteContentDbContext>((serviceProvider, options) => options.UseSqlServer(connectionString)
            .AddInterceptors(serviceProvider.GetRequiredService<TenantSaveChangesInterceptor>()));

        services.AddScoped<IAboutUsRepository, AboutUsRepository>();
        services.AddScoped<IFooterInfoRepository, FooterInfoRepository>();
        services.AddScoped<IFeatureHighlightRepository, FeatureHighlightRepository>();
        services.AddScoped<IPageBannerRepository, PageBannerRepository>();
        services.AddScoped<IHeroSlideRepository, HeroSlideRepository>();
        services.AddScoped<ITestimonialRepository, TestimonialRepository>();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AssemblyMarker).Assembly));
        services.AddValidatorsFromAssembly(typeof(AssemblyMarker).Assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(SiteContentTransactionAddingBehavior<,>));

        return services;
    }
}
