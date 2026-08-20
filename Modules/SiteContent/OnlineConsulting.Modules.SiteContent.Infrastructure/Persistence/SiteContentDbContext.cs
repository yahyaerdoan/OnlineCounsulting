using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.SiteContent.Domain;
using OnlineConsulting.Modules.SiteContent.Domain.Gallery;
using OnlineConsulting.Modules.SiteContent.Domain.Partnerships;
using OnlineConsulting.Modules.SiteContent.Domain.Service;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.SiteContent.Infrastructure.Persistence;

public class SiteContentDbContext(DbContextOptions<SiteContentDbContext> options, ITenantProvider tenantProvider) : DbContext(options)
{
    public DbSet<AboutUs> AboutUss => Set<AboutUs>();
    public DbSet<FooterInfo> FooterInfos => Set<FooterInfo>();
    public DbSet<FeatureHighlight> FeatureHighlights => Set<FeatureHighlight>();
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _ = modelBuilder.HasDefaultSchema("SiteContent");

        _ = modelBuilder.Entity<AboutUs>(builder =>
        {
            _ = builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
            _ = builder.Property(x => x.Description).HasMaxLength(4000).IsRequired();
            _ = builder.Property(x => x.CoverImage).HasMaxLength(500);
            _ = builder.Property(x => x.VideoUrl).HasMaxLength(500);
            _ = builder.Property(x => x.RowVersion).IsRowVersion();
            _ = builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
        });

        _ = modelBuilder.Entity<FooterInfo>(builder =>
        {
            _ = builder.Property(x => x.ImageUrl).HasMaxLength(500).IsRequired();
            _ = builder.Property(x => x.Description).HasMaxLength(2000).IsRequired();
            _ = builder.Property(x => x.RowVersion).IsRowVersion();
            _ = builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
        });

        _ = modelBuilder.Entity<FeatureHighlight>(builder =>
        {
            _ = builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
            _ = builder.Property(x => x.Description).HasMaxLength(2000).IsRequired();
            _ = builder.Property(x => x.ImageUrl).HasMaxLength(500).IsRequired();
            _ = builder.Property(x => x.RowVersion).IsRowVersion();
            _ = builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
        });

        _ = modelBuilder.Entity<PageBanner>(builder =>
        {
            _ = builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
            _ = builder.Property(x => x.Description).HasMaxLength(2000).IsRequired();
            _ = builder.Property(x => x.ImageUrl).HasMaxLength(500).IsRequired();
            _ = builder.Property(x => x.RowVersion).IsRowVersion();
            _ = builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
        });

        _ = modelBuilder.Entity<HeroSlide>(builder =>
        {
            _ = builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
            _ = builder.Property(x => x.Description).HasMaxLength(2000).IsRequired();
            _ = builder.Property(x => x.ImageUrl).HasMaxLength(500).IsRequired();
            _ = builder.Property(x => x.RowVersion).IsRowVersion();
            _ = builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
        });

        _ = modelBuilder.Entity<Testimonial>(builder =>
        {
            _ = builder.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
            _ = builder.Property(x => x.LastName).HasMaxLength(100).IsRequired();
            _ = builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
            _ = builder.Property(x => x.Description).HasMaxLength(2000).IsRequired();
            _ = builder.Property(x => x.ImageUrl).HasMaxLength(500).IsRequired();
            _ = builder.Property(x => x.RowVersion).IsRowVersion();
            _ = builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
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
            _ = builder.Property(x => x.RowVersion).IsRowVersion();
            _ = builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
        });

        _ = modelBuilder.Entity<PartnershipSocialLink>(builder =>
        {
            _ = builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
            _ = builder.Property(x => x.Url).HasMaxLength(500).IsRequired();
            _ = builder.Property(x => x.Icon).HasMaxLength(2000).IsRequired();
            _ = builder.Property(x => x.IconColor).HasMaxLength(7);
            _ = builder.Property(x => x.RowVersion).IsRowVersion();
            _ = builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
        });

        _ = modelBuilder.Entity<GalleryCategory>(builder =>
        {
            _ = builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
            _ = builder.Property(x => x.Description).HasMaxLength(500);
            _ = builder.Property(x => x.RowVersion).IsRowVersion();
            _ = builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
        });

        _ = modelBuilder.Entity<GalleryItem>(builder =>
        {
            _ = builder.Property(x => x.Description).HasMaxLength(2000).IsRequired();
            _ = builder.Property(x => x.RowVersion).IsRowVersion();
            _ = builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
        });

        _ = modelBuilder.Entity<GalleryItemCategory>(builder =>
        {
            _ = builder.HasIndex(x => new { x.TenantId, x.GalleryItemId, x.GalleryCategoryId }).IsUnique();
            _ = builder.Property(x => x.RowVersion).IsRowVersion();
            _ = builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
        });

        _ = modelBuilder.Entity<ServiceProcessStep>(builder =>
        {
            _ = builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
            _ = builder.Property(x => x.Description).HasMaxLength(2000).IsRequired();
            _ = builder.Property(x => x.Icon).HasMaxLength(2000).IsRequired();
            _ = builder.Property(x => x.IconColor).HasMaxLength(7);
            _ = builder.Property(x => x.RowVersion).IsRowVersion();
            _ = builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
        });

        _ = modelBuilder.Entity<ServiceOffering>(builder =>
        {
            _ = builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
            _ = builder.Property(x => x.Description).HasMaxLength(2000).IsRequired();
            _ = builder.Property(x => x.Icon).HasMaxLength(2000).IsRequired();
            _ = builder.Property(x => x.IconColor).HasMaxLength(7);
            _ = builder.Property(x => x.RowVersion).IsRowVersion();
            _ = builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
        });

        _ = modelBuilder.Entity<SocialLink>(builder =>
        {
            _ = builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
            _ = builder.Property(x => x.Url).HasMaxLength(500).IsRequired();
            _ = builder.Property(x => x.Icon).HasMaxLength(2000).IsRequired();
            _ = builder.Property(x => x.IconColor).HasMaxLength(7);
            _ = builder.Property(x => x.RowVersion).IsRowVersion();
            _ = builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
        });

        _ = modelBuilder.Entity<ServiceArea>(builder =>
        {
            _ = builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
            _ = builder.Property(x => x.State).HasMaxLength(50).IsRequired();
            _ = builder.Property(x => x.Slug).HasMaxLength(120).IsRequired();
            _ = builder.Property(x => x.IntroText).HasMaxLength(2000);
            _ = builder.Property(x => x.RowVersion).IsRowVersion();
            _ = builder.HasIndex(x => x.Slug).IsUnique();
            _ = builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
        });

        _ = modelBuilder.Entity<FaqItem>(builder =>
        {
            _ = builder.Property(x => x.Question).HasMaxLength(300).IsRequired();
            _ = builder.Property(x => x.Answer).HasMaxLength(2000).IsRequired();
            _ = builder.Property(x => x.RowVersion).IsRowVersion();
            _ = builder.HasIndex(x => x.ServiceId);
            _ = builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
        });

        _ = modelBuilder.Entity<Promotion>(builder =>
        {
            _ = builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
            _ = builder.Property(x => x.Description).HasMaxLength(2000).IsRequired();
            _ = builder.Property(x => x.CtaText).HasMaxLength(100);
            _ = builder.Property(x => x.CtaUrl).HasMaxLength(500);
            _ = builder.Property(x => x.RowVersion).IsRowVersion();
            _ = builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
        });

        base.OnModelCreating(modelBuilder);
    }
}
