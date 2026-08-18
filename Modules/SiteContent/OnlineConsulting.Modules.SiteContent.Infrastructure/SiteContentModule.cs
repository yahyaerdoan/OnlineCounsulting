using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnlineConsulting.Modules.SiteContent.Application;
using OnlineConsulting.Modules.SiteContent.Application.Features.AboutUss.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.FaqItems.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.FeatureHighlights.Abstractions;
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
using OnlineConsulting.Modules.SiteContent.Infrastructure.Pipelines;
using OnlineConsulting.Modules.SiteContent.Infrastructure.Repositories;
using OnlineConsulting.SharedKernel.Auditing;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.SiteContent.Infrastructure;

public static class SiteContentModule
{
    public static IServiceCollection AddSiteContentModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetSection("OnlineConsultingDbConnections:DevelopmentDbConnection").Value;

        services.AddScoped<TenantSaveChangesInterceptor>();
        services.AddScoped<AuditSaveChangesInterceptor>();

        services.AddDbContext<SiteContentDbContext>((serviceProvider, options) => options.UseSqlServer(connectionString)
            .AddInterceptors(serviceProvider.GetRequiredService<TenantSaveChangesInterceptor>(), serviceProvider.GetRequiredService<AuditSaveChangesInterceptor>()));

        services.AddScoped<IAboutUsRepository, AboutUsRepository>();
        services.AddScoped<IFooterInfoRepository, FooterInfoRepository>();
        services.AddScoped<IFeatureHighlightRepository, FeatureHighlightRepository>();
        services.AddScoped<IPageBannerRepository, PageBannerRepository>();
        services.AddScoped<IHeroSlideRepository, HeroSlideRepository>();
        services.AddScoped<ITestimonialRepository, TestimonialRepository>();
        services.AddScoped<IPartnershipRepository, PartnershipRepository>();
        services.AddScoped<IPartnershipSocialLinkRepository, PartnershipSocialLinkRepository>();
        services.AddScoped<IGalleryCategoryRepository, GalleryCategoryRepository>();
        services.AddScoped<IGalleryItemRepository, GalleryItemRepository>();
        services.AddScoped<IGalleryItemCategoryRepository, GalleryItemCategoryRepository>();
        services.AddScoped<IServiceProcessStepRepository, ServiceProcessStepRepository>();
        services.AddScoped<IServiceOfferingRepository, ServiceOfferingRepository>();
        services.AddScoped<ISocialLinkRepository, SocialLinkRepository>();
        services.AddScoped<IServiceAreaRepository, ServiceAreaRepository>();
        services.AddScoped<IFaqItemRepository, FaqItemRepository>();
        services.AddScoped<IPromotionRepository, PromotionRepository>();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AssemblyMarker).Assembly));
        services.AddValidatorsFromAssembly(typeof(AssemblyMarker).Assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(SiteContentTransactionAddingBehavior<,>));

        return services;
    }
}
