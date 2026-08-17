using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.SiteContent.Domain;
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("SiteContent");

        modelBuilder.Entity<AboutUs>(builder =>
        {
            builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(4000).IsRequired();
            builder.Property(x => x.CoverImage).HasMaxLength(500);
            builder.Property(x => x.VideoUrl).HasMaxLength(500);
            builder.Property(x => x.RowVersion).IsRowVersion();
            builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
        });

        modelBuilder.Entity<FooterInfo>(builder =>
        {
            builder.Property(x => x.ImageUrl).HasMaxLength(500).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(2000).IsRequired();
            builder.Property(x => x.RowVersion).IsRowVersion();
            builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
        });

        modelBuilder.Entity<FeatureHighlight>(builder =>
        {
            builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(2000).IsRequired();
            builder.Property(x => x.ImageUrl).HasMaxLength(500).IsRequired();
            builder.Property(x => x.RowVersion).IsRowVersion();
            builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
        });

        modelBuilder.Entity<PageBanner>(builder =>
        {
            builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(2000).IsRequired();
            builder.Property(x => x.ImageUrl).HasMaxLength(500).IsRequired();
            builder.Property(x => x.RowVersion).IsRowVersion();
            builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
        });

        modelBuilder.Entity<HeroSlide>(builder =>
        {
            builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(2000).IsRequired();
            builder.Property(x => x.ImageUrl).HasMaxLength(500).IsRequired();
            builder.Property(x => x.RowVersion).IsRowVersion();
            builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
        });

        modelBuilder.Entity<Testimonial>(builder =>
        {
            builder.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
            builder.Property(x => x.LastName).HasMaxLength(100).IsRequired();
            builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(2000).IsRequired();
            builder.Property(x => x.ImageUrl).HasMaxLength(500).IsRequired();
            builder.Property(x => x.RowVersion).IsRowVersion();
            builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
        });

        modelBuilder.Entity<Partnership>(builder =>
        {
            builder.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
            builder.Property(x => x.LastName).HasMaxLength(100).IsRequired();
            builder.Property(x => x.Email).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
            builder.Property(x => x.CompanyName).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(2000).IsRequired();
            builder.Property(x => x.WebsiteUrl).HasMaxLength(500).IsRequired();
            builder.Property(x => x.RowVersion).IsRowVersion();
            builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
        });

        modelBuilder.Entity<PartnershipSocialLink>(builder =>
        {
            builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
            builder.Property(x => x.Url).HasMaxLength(500).IsRequired();
            builder.Property(x => x.Icon).HasMaxLength(2000).IsRequired();
            builder.Property(x => x.IconColor).HasMaxLength(7);
            builder.Property(x => x.RowVersion).IsRowVersion();
            builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
        });

        modelBuilder.Entity<GalleryCategory>(builder =>
        {
            builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(500);
            builder.Property(x => x.RowVersion).IsRowVersion();
            builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
        });

        modelBuilder.Entity<GalleryItem>(builder =>
        {
            builder.Property(x => x.Description).HasMaxLength(2000).IsRequired();
            builder.Property(x => x.RowVersion).IsRowVersion();
            builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
        });

        modelBuilder.Entity<GalleryItemCategory>(builder =>
        {
            builder.HasIndex(x => new { x.TenantId, x.GalleryItemId, x.GalleryCategoryId }).IsUnique();
            builder.Property(x => x.RowVersion).IsRowVersion();
            builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
        });

        modelBuilder.Entity<ServiceProcessStep>(builder =>
        {
            builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(2000).IsRequired();
            builder.Property(x => x.Icon).HasMaxLength(2000).IsRequired();
            builder.Property(x => x.IconColor).HasMaxLength(7);
            builder.Property(x => x.RowVersion).IsRowVersion();
            builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
        });

        modelBuilder.Entity<ServiceOffering>(builder =>
        {
            builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(2000).IsRequired();
            builder.Property(x => x.Icon).HasMaxLength(2000).IsRequired();
            builder.Property(x => x.IconColor).HasMaxLength(7);
            builder.Property(x => x.RowVersion).IsRowVersion();
            builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
        });

        modelBuilder.Entity<SocialLink>(builder =>
        {
            builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
            builder.Property(x => x.Url).HasMaxLength(500).IsRequired();
            builder.Property(x => x.Icon).HasMaxLength(2000).IsRequired();
            builder.Property(x => x.IconColor).HasMaxLength(7);
            builder.Property(x => x.RowVersion).IsRowVersion();
            builder.ApplyTenantAndSoftDeleteFilter(tenantProvider);
        });

        base.OnModelCreating(modelBuilder);
    }
}
