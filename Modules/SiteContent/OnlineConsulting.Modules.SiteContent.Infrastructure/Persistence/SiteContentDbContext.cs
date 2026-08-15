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
            builder.HasQueryFilter(x => x.TenantId == tenantProvider.TenantId && x.DeletedDate == null);
        });

        modelBuilder.Entity<FooterInfo>(builder =>
        {
            builder.Property(x => x.ImageUrl).HasMaxLength(500).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(2000).IsRequired();
            builder.Property(x => x.RowVersion).IsRowVersion();
            builder.HasQueryFilter(x => x.TenantId == tenantProvider.TenantId && x.DeletedDate == null);
        });

        modelBuilder.Entity<FeatureHighlight>(builder =>
        {
            builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(2000).IsRequired();
            builder.Property(x => x.ImageUrl).HasMaxLength(500).IsRequired();
            builder.Property(x => x.RowVersion).IsRowVersion();
            builder.HasQueryFilter(x => x.TenantId == tenantProvider.TenantId && x.DeletedDate == null);
        });

        modelBuilder.Entity<PageBanner>(builder =>
        {
            builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(2000).IsRequired();
            builder.Property(x => x.ImageUrl).HasMaxLength(500).IsRequired();
            builder.Property(x => x.RowVersion).IsRowVersion();
            builder.HasQueryFilter(x => x.TenantId == tenantProvider.TenantId && x.DeletedDate == null);
        });

        modelBuilder.Entity<HeroSlide>(builder =>
        {
            builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(2000).IsRequired();
            builder.Property(x => x.ImageUrl).HasMaxLength(500).IsRequired();
            builder.Property(x => x.RowVersion).IsRowVersion();
            builder.HasQueryFilter(x => x.TenantId == tenantProvider.TenantId && x.DeletedDate == null);
        });

        modelBuilder.Entity<Testimonial>(builder =>
        {
            builder.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
            builder.Property(x => x.LastName).HasMaxLength(100).IsRequired();
            builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(2000).IsRequired();
            builder.Property(x => x.ImageUrl).HasMaxLength(500).IsRequired();
            builder.Property(x => x.RowVersion).IsRowVersion();
            builder.HasQueryFilter(x => x.TenantId == tenantProvider.TenantId && x.DeletedDate == null);
        });

        base.OnModelCreating(modelBuilder);
    }
}
