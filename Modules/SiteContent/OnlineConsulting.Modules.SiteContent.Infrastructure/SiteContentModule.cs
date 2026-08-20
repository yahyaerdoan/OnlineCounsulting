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
using OnlineConsulting.Modules.SiteContent.Infrastructure.Repositories.Gallery;
using OnlineConsulting.Modules.SiteContent.Infrastructure.Repositories.Partnerships;
using OnlineConsulting.Modules.SiteContent.Infrastructure.Repositories.Service;
using OnlineConsulting.SharedKernel.Auditing;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.SiteContent.Infrastructure;

public static class SiteContentModule
{
    public static IServiceCollection AddSiteContentModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        _ = services.AddScoped<TenantSaveChangesInterceptor>();
        _ = services.AddScoped<AuditSaveChangesInterceptor>();

        _ = services.AddDbContext<SiteContentDbContext>((serviceProvider, options) => options.UseSqlServer(connectionString)
            .AddInterceptors(serviceProvider.GetRequiredService<TenantSaveChangesInterceptor>(), serviceProvider.GetRequiredService<AuditSaveChangesInterceptor>()));

        _ = services.AddScoped<IAboutUsRepository, AboutUsRepository>();
        _ = services.AddScoped<IFooterInfoRepository, FooterInfoRepository>();
        _ = services.AddScoped<IFeatureHighlightRepository, FeatureHighlightRepository>();
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
        _ = services.AddScoped<IFaqItemRepository, FaqItemRepository>();
        _ = services.AddScoped<IPromotionRepository, PromotionRepository>();

        _ = services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AssemblyMarker).Assembly));
        _ = services.AddValidatorsFromAssembly(typeof(AssemblyMarker).Assembly);
        _ = services.AddTransient(typeof(IPipelineBehavior<,>), typeof(SiteContentTransactionAddingBehavior<,>));

        return services;
    }
}
