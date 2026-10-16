using Core.PersistenceLayer.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.SiteContent.Domain;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Domain.Gallery;
using OnlineConsulting.Modules.SiteContent.Domain.Landing;
using OnlineConsulting.Modules.SiteContent.Domain.Legal;
using OnlineConsulting.Modules.SiteContent.Domain.Partnerships;
using OnlineConsulting.Modules.SiteContent.Domain.Service;

namespace OnlineConsulting.Modules.SiteContent.Infrastructure.Persistence;

/// <summary>EF Core context for the SiteContent module. Applies tenant/soft-delete filtering and per-entity property lengths; enforces per-tenant uniqueness on live ServiceArea slugs and the GalleryItemCategory (TenantId, GalleryItemId, GalleryCategoryId) combination, and indexes FaqItem.ServiceId. AboutUs.Description is sized for rich-text HTML from MudExRichTextEdit rather than plain text.</summary>
public class SiteContentDbContext(DbContextOptions<SiteContentDbContext> options, ITenantContext tenantContext) : TenantDbContext(options, tenantContext)
{
    public DbSet<AboutUs> AboutUss => Set<AboutUs>();
    public DbSet<FooterInfo> FooterInfos => Set<FooterInfo>();
    public DbSet<FeatureHighlight> FeatureHighlights => Set<FeatureHighlight>();
    public DbSet<FeatureHighlightsIntro> FeatureHighlightsIntros => Set<FeatureHighlightsIntro>();
    public DbSet<PageBanner> PageBanners => Set<PageBanner>();
    public DbSet<HeroSlide> HeroSlides => Set<HeroSlide>();
    public DbSet<Testimonial> Testimonials => Set<Testimonial>();
    public DbSet<Partnership> Partnerships => Set<Partnership>();
    public DbSet<PartnershipSocialLink> PartnershipSocialLinks => Set<PartnershipSocialLink>();
    public DbSet<GalleryCategory> GalleryCategories => Set<GalleryCategory>();
    public DbSet<GalleryItem> GalleryItems => Set<GalleryItem>();
    public DbSet<GalleryItemCategory> GalleryItemCategories => Set<GalleryItemCategory>();
    public DbSet<ServiceProcessStep> ServiceProcessSteps => Set<ServiceProcessStep>();
    public DbSet<ServiceOffering> ServiceOfferings => Set<ServiceOffering>();
    public DbSet<SocialLink> SocialLinks => Set<SocialLink>();
    public DbSet<ServiceArea> ServiceAreas => Set<ServiceArea>();
    public DbSet<FaqItem> FaqItems => Set<FaqItem>();
    public DbSet<Promotion> Promotions => Set<Promotion>();
    public DbSet<LandingPage> LandingPages => Set<LandingPage>();
    public DbSet<LegalPolicy> LegalPolicies => Set<LegalPolicy>();
    public DbSet<LandingBlock> LandingBlocks => Set<LandingBlock>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _ = modelBuilder.HasDefaultSchema("SiteContent");

        _ = modelBuilder.Entity<AboutUs>(builder =>
        {
            _ = builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
            _ = builder.Property(x => x.Description).HasMaxLength(20000).IsRequired();
            _ = builder.Property(x => x.CoverImage).HasMaxLength(500);
            _ = builder.Property(x => x.VideoUrl).HasMaxLength(500);
            _ = builder.Property(x => x.RowVersion).IsRowVersion();
        });

        _ = modelBuilder.Entity<FooterInfo>(builder =>
        {
            _ = builder.Property(x => x.ImageUrl).HasMaxLength(500).IsRequired();
            _ = builder.Property(x => x.Description).HasMaxLength(2000).IsRequired();
            _ = builder.Property(x => x.RowVersion).IsRowVersion();
        });

        _ = modelBuilder.Entity<FeatureHighlight>(builder =>
        {
            _ = builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
            _ = builder.Property(x => x.Description).HasMaxLength(2000).IsRequired();
            _ = builder.Property(x => x.ImageUrl).HasMaxLength(500).IsRequired();
            _ = builder.Property(x => x.RowVersion).IsRowVersion();
        });

        _ = modelBuilder.Entity<FeatureHighlightsIntro>(builder =>
        {
            _ = builder.Property(x => x.Description).HasMaxLength(1000).IsRequired();
            _ = builder.Property(x => x.RowVersion).IsRowVersion();
        });

        _ = modelBuilder.Entity<PageBanner>(builder =>
        {
            _ = builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
            _ = builder.Property(x => x.Description).HasMaxLength(2000).IsRequired();
            _ = builder.Property(x => x.ImageUrl).HasMaxLength(500).IsRequired();
            _ = builder.Property(x => x.RowVersion).IsRowVersion();
        });

        _ = modelBuilder.Entity<HeroSlide>(builder =>
        {
            _ = builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
            _ = builder.Property(x => x.Description).HasMaxLength(2000).IsRequired();
            _ = builder.Property(x => x.ImageUrl).HasMaxLength(500).IsRequired();
            _ = builder.Property(x => x.RowVersion).IsRowVersion();
        });

        _ = modelBuilder.Entity<Testimonial>(builder =>
        {
            _ = builder.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
            _ = builder.Property(x => x.LastName).HasMaxLength(100).IsRequired();
            _ = builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
            _ = builder.Property(x => x.Description).HasMaxLength(2000).IsRequired();
            _ = builder.Property(x => x.ImageUrl).HasMaxLength(500).IsRequired();
            _ = builder.Property(x => x.RowVersion).IsRowVersion();
        });

        _ = modelBuilder.Entity<Partnership>(builder =>
        {
            _ = builder.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
            _ = builder.Property(x => x.LastName).HasMaxLength(100).IsRequired();
            _ = builder.Property(x => x.Email).HasMaxLength(200).IsRequired();
            _ = builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
            _ = builder.Property(x => x.CompanyName).HasMaxLength(200).IsRequired();
            _ = builder.Property(x => x.Description).HasMaxLength(2000).IsRequired();
            _ = builder.Property(x => x.WebsiteUrl).HasMaxLength(500).IsRequired();
            _ = builder.Property(x => x.Kind).HasMaxLength(20).IsRequired().HasDefaultValue(PartnershipKinds.Partner);
            _ = builder.Property(x => x.RowVersion).IsRowVersion();
        });

        _ = modelBuilder.Entity<PartnershipSocialLink>(builder =>
        {
            _ = builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
            _ = builder.Property(x => x.Url).HasMaxLength(500).IsRequired();
            _ = builder.Property(x => x.Icon).HasMaxLength(2000).IsRequired();
            _ = builder.Property(x => x.IconColor).HasMaxLength(7);
            _ = builder.Property(x => x.RowVersion).IsRowVersion();
        });

        _ = modelBuilder.Entity<GalleryCategory>(builder =>
        {
            _ = builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
            _ = builder.Property(x => x.Description).HasMaxLength(500);
            _ = builder.Property(x => x.RowVersion).IsRowVersion();
        });

        _ = modelBuilder.Entity<GalleryItem>(builder =>
        {
            _ = builder.Property(x => x.Description).HasMaxLength(2000).IsRequired();
            _ = builder.Property(x => x.RowVersion).IsRowVersion();
            _ = builder.HasMany(x => x.Categories).WithOne().HasForeignKey(c => c.GalleryItemId).OnDelete(DeleteBehavior.Cascade);
            _ = builder.Ignore(x => x.CategoryIds);
        });

        _ = modelBuilder.Entity<GalleryItemCategory>(builder =>
        {
            _ = builder.HasIndex(x => new { x.TenantId, x.GalleryItemId, x.GalleryCategoryId }).IsUnique().HasFilter("[DeletedDate] IS NULL");
            _ = builder.Property(x => x.RowVersion).IsRowVersion();
        });

        _ = modelBuilder.Entity<ServiceProcessStep>(builder =>
        {
            _ = builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
            _ = builder.Property(x => x.Description).HasMaxLength(2000).IsRequired();
            _ = builder.Property(x => x.Icon).HasMaxLength(2000).IsRequired();
            _ = builder.Property(x => x.IconColor).HasMaxLength(7);
            _ = builder.Property(x => x.RowVersion).IsRowVersion();
        });

        _ = modelBuilder.Entity<ServiceOffering>(builder =>
        {
            _ = builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
            _ = builder.Property(x => x.Description).HasMaxLength(2000).IsRequired();
            _ = builder.Property(x => x.Icon).HasMaxLength(2000).IsRequired();
            _ = builder.Property(x => x.IconColor).HasMaxLength(7);
            _ = builder.Property(x => x.RowVersion).IsRowVersion();
        });

        _ = modelBuilder.Entity<SocialLink>(builder =>
        {
            _ = builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
            _ = builder.Property(x => x.Url).HasMaxLength(500).IsRequired();
            _ = builder.Property(x => x.Icon).HasMaxLength(2000).IsRequired();
            _ = builder.Property(x => x.IconColor).HasMaxLength(7);
            _ = builder.Property(x => x.RowVersion).IsRowVersion();
        });

        _ = modelBuilder.Entity<ServiceArea>(builder =>
        {
            _ = builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
            _ = builder.Property(x => x.State).HasMaxLength(50).IsRequired();
            _ = builder.Property(x => x.Slug).HasMaxLength(120).IsRequired();
            _ = builder.Property(x => x.IntroText).HasMaxLength(2000);
            _ = builder.Property(x => x.RowVersion).IsRowVersion();
            _ = builder.HasIndex(x => new { x.TenantId, x.Slug }).IsUnique().HasFilter("[DeletedDate] IS NULL");
        });

        _ = modelBuilder.Entity<FaqItem>(builder =>
        {
            _ = builder.Property(x => x.Question).HasMaxLength(300).IsRequired();
            _ = builder.Property(x => x.Answer).HasMaxLength(2000).IsRequired();
            _ = builder.Property(x => x.RowVersion).IsRowVersion();
            _ = builder.HasIndex(x => x.ServiceId);
        });

        _ = modelBuilder.Entity<Promotion>(builder =>
        {
            _ = builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
            _ = builder.Property(x => x.Description).HasMaxLength(2000).IsRequired();
            _ = builder.Property(x => x.CtaText).HasMaxLength(100);
            _ = builder.Property(x => x.CtaUrl).HasMaxLength(500);
            _ = builder.Property(x => x.RowVersion).IsRowVersion();
        });

        _ = modelBuilder.Entity<LegalPolicy>(builder =>
        {
            _ = builder.Property(x => x.PrivacyPolicy).HasMaxLength(LegalLimits.Document);
            _ = builder.Property(x => x.TermsOfService).HasMaxLength(LegalLimits.Document);
            _ = builder.Property(x => x.RowVersion).IsRowVersion();
        });

        _ = modelBuilder.Entity<LandingPage>(builder =>
        {
            _ = builder.Property(x => x.HeroEyebrow).HasMaxLength(LandingLimits.Eyebrow).IsRequired();
            _ = builder.Property(x => x.HeroTitle).HasMaxLength(LandingLimits.Title).IsRequired();
            _ = builder.Property(x => x.HeroLead).HasMaxLength(LandingLimits.Lead).IsRequired();
            _ = builder.Property(x => x.PrimaryCtaText).HasMaxLength(LandingLimits.Cta).IsRequired();
            _ = builder.Property(x => x.SecondaryCtaText).HasMaxLength(LandingLimits.Cta).IsRequired();
            _ = builder.Property(x => x.FeaturesEyebrow).HasMaxLength(LandingLimits.Eyebrow).IsRequired();
            _ = builder.Property(x => x.FeaturesTitle).HasMaxLength(LandingLimits.Title).IsRequired();
            _ = builder.Property(x => x.FeaturesLead).HasMaxLength(LandingLimits.Lead);
            _ = builder.Property(x => x.StepsEyebrow).HasMaxLength(LandingLimits.Eyebrow).IsRequired();
            _ = builder.Property(x => x.StepsTitle).HasMaxLength(LandingLimits.Title).IsRequired();
            _ = builder.Property(x => x.StepsLead).HasMaxLength(LandingLimits.Lead);
            _ = builder.Property(x => x.PricingEyebrow).HasMaxLength(LandingLimits.Eyebrow).IsRequired();
            _ = builder.Property(x => x.PricingTitle).HasMaxLength(LandingLimits.Title).IsRequired();
            _ = builder.Property(x => x.PricingLead).HasMaxLength(LandingLimits.Lead);
            _ = builder.Property(x => x.FaqEyebrow).HasMaxLength(LandingLimits.Eyebrow).IsRequired();
            _ = builder.Property(x => x.FaqTitle).HasMaxLength(LandingLimits.Title).IsRequired();
            _ = builder.Property(x => x.ClosingTitle).HasMaxLength(LandingLimits.Title).IsRequired();
            _ = builder.Property(x => x.ClosingLead).HasMaxLength(LandingLimits.Lead);
            _ = builder.Property(x => x.AboutHeroEyebrow).HasMaxLength(LandingLimits.Eyebrow);
            _ = builder.Property(x => x.AboutHeroTitle).HasMaxLength(LandingLimits.Title);
            _ = builder.Property(x => x.AboutHeroLead).HasMaxLength(LandingLimits.Lead);
            _ = builder.Property(x => x.IndustriesEyebrow).HasMaxLength(LandingLimits.Eyebrow);
            _ = builder.Property(x => x.IndustriesTitle).HasMaxLength(LandingLimits.Title);
            _ = builder.Property(x => x.IndustriesLead).HasMaxLength(LandingLimits.Lead);
            _ = builder.Property(x => x.ContactHeroEyebrow).HasMaxLength(LandingLimits.Eyebrow);
            _ = builder.Property(x => x.ContactHeroTitle).HasMaxLength(LandingLimits.Title);
            _ = builder.Property(x => x.ContactHeroLead).HasMaxLength(LandingLimits.Lead);
            _ = builder.Property(x => x.HeroImageUrl).HasMaxLength(LandingLimits.ImageUrl);
            _ = builder.Property(x => x.FeaturesHeroImageUrl).HasMaxLength(LandingLimits.ImageUrl);
            _ = builder.Property(x => x.IndustriesHeroImageUrl).HasMaxLength(LandingLimits.ImageUrl);
            _ = builder.Property(x => x.AboutHeroImageUrl).HasMaxLength(LandingLimits.ImageUrl);
            _ = builder.Property(x => x.ContactHeroImageUrl).HasMaxLength(LandingLimits.ImageUrl);
            _ = builder.Property(x => x.RowVersion).IsRowVersion();
        });

        _ = modelBuilder.Entity<LandingBlock>(builder =>
        {
            _ = builder.Property(x => x.Section).HasMaxLength(LandingLimits.Section).IsRequired();
            _ = builder.Property(x => x.Title).HasMaxLength(LandingLimits.Title).IsRequired();
            _ = builder.Property(x => x.Body).HasMaxLength(LandingLimits.Body).IsRequired();
            _ = builder.Property(x => x.Icon).HasMaxLength(LandingLimits.Icon);
            _ = builder.Property(x => x.RowVersion).IsRowVersion();
        });

        base.OnModelCreating(modelBuilder);
    }
}
