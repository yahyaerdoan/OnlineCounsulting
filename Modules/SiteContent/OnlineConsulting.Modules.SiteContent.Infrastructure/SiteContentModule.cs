using Core.ApplicationLayer.Pipelines.Transactions.Extensions;
using Core.PersistenceLayer.Repositories.Auditing;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnlineConsulting.Modules.SiteContent.Application;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.AboutUss.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.FaqItems.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.FeatureHighlights.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.FeatureHighlightsIntros.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.FooterInfos.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.GalleryCategories.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.GalleryItems.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.HeroSlides.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.PageBanners.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.Partnerships.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.PartnershipSocialLinks.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.Promotions.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceAreas.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceOfferings.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceProcessSteps.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.SocialLinks.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.Testimonials.Abstractions;
using OnlineConsulting.Modules.SiteContent.Infrastructure.Persistence;
using OnlineConsulting.Modules.SiteContent.Infrastructure.Repositories;
using OnlineConsulting.Modules.SiteContent.Infrastructure.Repositories.Gallery;
using OnlineConsulting.Modules.SiteContent.Infrastructure.Repositories.Partnerships;
using OnlineConsulting.Modules.SiteContent.Infrastructure.Repositories.Service;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.Tenancy;
using OnlineConsulting.SharedKernel.Transactions;

namespace OnlineConsulting.Modules.SiteContent.Infrastructure;

/// <summary>DI composition root for the SiteContent module - registers its DbContext, repositories, MediatR handlers, validators, and operation claims.</summary>
public static class SiteContentModule
{
    /// <summary>Wires up the SiteContent module's EF Core context, repositories, MediatR/FluentValidation, transaction pipeline, and default admin permissions.</summary>
    public static IServiceCollection AddSiteContentModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        _ = services.AddScoped<TenantSaveChangesInterceptor>();

        _ = services.AddDbContext<SiteContentDbContext>((serviceProvider, options) => options.UseSqlServer(connectionString)
            .AddInterceptors(serviceProvider.GetRequiredService<TenantSaveChangesInterceptor>(), serviceProvider.GetRequiredService<AuditSaveChangesInterceptor>()));

        _ = services.AddScoped<IAboutUsRepository, AboutUsRepository>();
        _ = services.AddScoped<IFooterInfoRepository, FooterInfoRepository>();
        _ = services.AddScoped<IFeatureHighlightRepository, FeatureHighlightRepository>();
        _ = services.AddScoped<IFeatureHighlightsIntroRepository, FeatureHighlightsIntroRepository>();
        _ = services.AddScoped<IPageBannerRepository, PageBannerRepository>();
        _ = services.AddScoped<IHeroSlideRepository, HeroSlideRepository>();
        _ = services.AddScoped<ITestimonialRepository, TestimonialRepository>();
        _ = services.AddScoped<IPartnershipRepository, PartnershipRepository>();
        _ = services.AddScoped<IPartnershipSocialLinkRepository, PartnershipSocialLinkRepository>();
        _ = services.AddScoped<IGalleryCategoryRepository, GalleryCategoryRepository>();
        _ = services.AddScoped<IGalleryItemRepository, GalleryItemRepository>();
        _ = services.AddScoped<IGalleryItemCategoryRepository, GalleryItemCategoryRepository>();
        _ = services.AddScoped<IServiceProcessStepRepository, ServiceProcessStepRepository>();
        _ = services.AddScoped<IServiceOfferingRepository, ServiceOfferingRepository>();
        _ = services.AddScoped<ISocialLinkRepository, SocialLinkRepository>();
        _ = services.AddScoped<IServiceAreaRepository, ServiceAreaRepository>();
        _ = services.AddMemoryCache();
        _ = services.AddHttpClient(Geocoding.CityGeocoder.GeoapifyClient, client =>
        {
            client.BaseAddress = new Uri("https://api.geoapify.com/");
            client.Timeout = TimeSpan.FromSeconds(8);
        });
        _ = services.AddHttpClient(Geocoding.CityGeocoder.NominatimClient, client =>
        {
            client.BaseAddress = new Uri("https://nominatim.openstreetmap.org/");
            client.Timeout = TimeSpan.FromSeconds(8);
            client.DefaultRequestHeaders.UserAgent.ParseAdd("ComfortPro-ServiceAreas/1.0 (+https://comfortpro.example)");
        });
        _ = services.AddScoped<ICityGeocoder, Geocoding.CityGeocoder>();
        _ = services.AddHostedService<Geocoding.ServiceAreaCoordinatesBackfill>();
        _ = services.AddScoped<IFaqItemRepository, FaqItemRepository>();
        _ = services.AddScoped<IPromotionRepository, PromotionRepository>();

        _ = services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AssemblyMarker).Assembly));
        _ = services.AddValidatorsFromAssembly(typeof(AssemblyMarker).Assembly);
        _ = services.AddTransactionalDbContext<ISiteContentTransactionRequest, SiteContentDbContext>();

        _ = services.AddSingleton<IDefaultAdminPermissions>(new DefaultAdminPermissions(SiteContentOperationClaims.All));
        return services;
    }
}
