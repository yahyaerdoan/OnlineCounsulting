using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Entity.Concretions.BaseEntities;
using OnlineConsulting.Entity.Concretions.Entities;
namespace OnlineConsulting.DataAccess.Concretions.Contexts;

/// <summary>Plain DbContext for this module; Identity/User/Role now live in the Identity module's own AppIdentityDbContext, so a user is referenced only by UserId, never EF navigation.</summary>
public class OnlineConsultingDbContext(DbContextOptions<OnlineConsultingDbContext> options,
    IHttpContextAccessor httpContextAccessor) : DbContext(options)
{
    #region DbSets
    public DbSet<AboutUs>? AboutUs { get; set; }
    public DbSet<Book>? Books { get; set; }
    public DbSet<Category>? Categories { get; set; }
    public DbSet<Contact>? Contacts { get; set; }
    public DbSet<FooterAbout>? FooterAbouts { get; set; }
    public DbSet<HowIGetService>? HowIGetServices { get; set; }
    public DbSet<ClassIcon>? ClassIcons { get; set; }
    public DbSet<ImgIcon>? ImgIcons { get; set; }
    public DbSet<Message>? Messages { get; set; }
    public DbSet<Newsletter>? Newsletters { get; set; }
    public DbSet<Partnership>? Partnerships { get; set; }
    public DbSet<PartnershipSocialMedia>? PartnershipSocialMedias { get; set; }
    public DbSet<ProvidedItem>? ProvidedItems { get; set; }
    public DbSet<Service>? Services { get; set; }
    public DbSet<ServiceImage>? ServiceImages { get; set; }
    public DbSet<SliderItem>? SliderItems { get; set; }
    public DbSet<SocialMedia>? SocialMedias { get; set; }
    public DbSet<Testimonial>? Testimonials { get; set; }
    public DbSet<WhatWeProvide>? WhatWeProvides { get; set; }
    public DbSet<GalleryItem>? GalleryItems { get; set; }
    public DbSet<GalleryCategory>? GalleryCategories { get; set; }
    public DbSet<Breadcrumb>? Breadcrumbs { get; set; }
    public DbSet<GalleryItemCategory>? GalleryItemCategory { get; set; }
    public DbSet<UserAddress>? UserAddresses { get; set; }
    public DbSet<Order>? Orders { get; set; }
    public DbSet<OrderItem>? OrderItems { get; set; }

    #endregion

    #region OnModelCreating
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // EF Core already creates these indexes implicitly via convention; kept explicit to state the intent.
        modelBuilder.Entity<BasketItem>().HasIndex(x => x.ServiceId);
        modelBuilder.Entity<BasketItem>().HasIndex(x => x.BasketId);
        modelBuilder.Entity<Basket>().HasIndex(x => x.UserId);
        modelBuilder.Entity<Book>().HasIndex(x => x.ServiceId);
        modelBuilder.Entity<HowIGetService>().HasIndex(x => x.ImgIconId);
        modelBuilder.Entity<Category>().HasIndex(x => x.ImgIconId);
        modelBuilder.Entity<Order>().HasIndex(x => x.ShippingAddressId);
        modelBuilder.Entity<Order>().HasIndex(x => x.InvoiceAddressId);
        modelBuilder.Entity<Order>().HasIndex(x => x.UserId);
        modelBuilder.Entity<ServiceImage>().HasIndex(x => x.ServiceId);
        modelBuilder.Entity<OrderItem>().HasIndex(x => x.OrderId);
        modelBuilder.Entity<OrderItem>().HasIndex(x => x.ServiceId);
        modelBuilder.Entity<Service>().HasIndex(x => x.CategoryId);
        modelBuilder.Entity<ProvidedItem>().HasIndex(x => x.ImgIconId);
        modelBuilder.Entity<PartnershipSocialMedia>().HasIndex(x => x.ClassIconId);
        modelBuilder.Entity<PartnershipSocialMedia>().HasIndex(x => x.PartnershipId);
        modelBuilder.Entity<SocialMedia>().HasIndex(x => x.ClassIconId);
        modelBuilder.Entity<UserAddress>().HasIndex(x => x.UserId);

        modelBuilder.Entity<Order>()
            .HasOne(x => x.ShippingAddress)
            .WithMany(x => x.OrderShippingAddress)
            .HasForeignKey(x => x.ShippingAddressId)
            .OnDelete(DeleteBehavior.ClientSetNull);

        modelBuilder.Entity<Order>()
            .HasOne(x => x.InvoiceAddress)
            .WithMany(x => x.OrderInvoiceAddress)
            .HasForeignKey(x => x.InvoiceAddressId)
            .OnDelete(DeleteBehavior.ClientSetNull);

        modelBuilder.Entity<Service>()
            .HasOne(s => s.Category)
            .WithMany(c => c.Services)
            .HasForeignKey(s => s.CategoryId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<SocialMedia>()
            .HasOne(sm => sm.ClassIcon)
            .WithMany(ci => ci.SocialMedias)
            .HasForeignKey(sm => sm.ClassIconId);

        modelBuilder.Entity<PartnershipSocialMedia>()
            .HasOne(sm => sm.ClassIcon)
            .WithMany(ci => ci.PartnershipSocialMedias)
            .HasForeignKey(sm => sm.ClassIconId);

        modelBuilder.Entity<Category>()
            .HasOne(sm => sm.ImgIcon)
            .WithMany(ci => ci.Categories)
            .HasForeignKey(sm => sm.ImgIconId);

        modelBuilder.Entity<HowIGetService>()
            .HasOne(sm => sm.ImgIcon)
            .WithMany(ci => ci.HowIGetServices)
            .HasForeignKey(sm => sm.ImgIconId);

        modelBuilder.Entity<ProvidedItem>()
            .HasOne(sm => sm.ImgIcon)
            .WithMany(ci => ci.ProvidedItems)
            .HasForeignKey(sm => sm.ImgIconId);

        modelBuilder.Entity<Book>()
            .Property(b => b.TotalPrice)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Service>()
            .Property(s => s.Price)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Service>()
            .Property(s => s.DiscountedPrice)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<GalleryItemCategory>()
            .HasKey(sc => new
            {
                sc.GalleryItemsId,
                sc.GalleryCategoriesId
            });

        modelBuilder.Entity<GalleryItemCategory>()
            .HasOne(sc => sc.GalleryItem)
            .WithMany(c => c.GalleryCategories)
            .HasForeignKey(sc => sc.GalleryItemsId);

        modelBuilder.Entity<GalleryItemCategory>()
            .HasOne(sc => sc.GalleryCategory)
            .WithMany(c => c.GalleryCategories)
            .HasForeignKey(sc => sc.GalleryCategoriesId);

        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Order)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(oi => oi.OrderId);

        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Service)
            .WithMany(s => s.OrderItems)
            .HasForeignKey(oi => oi.ServiceId);

        modelBuilder.Entity<OrderItem>()
            .Property(oi => oi.UnitPrice)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<OrderItem>()
            .Property(oi => oi.SubTotalPrice)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<OrderItem>()
            .Property(oi => oi.TaxAmount)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<OrderItem>()
            .Property(oi => oi.TotalPrice)
            .HasColumnType("decimal(18,2)");

        base.OnModelCreating(modelBuilder); // must run last
    }

    #endregion

    #region SaveChangesAsync
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries()
            .Where(e =>
                e.Entity is BaseEntity &&
                (e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted));

        var currentUser = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Anonymous";
        var currentTime = DateTime.Now;

        foreach (var entry in entries)
        {
            if (entry.Entity is BaseEntity baseEntity)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        baseEntity.Status = true;
                        baseEntity.CreatedDate = currentTime;
                        baseEntity.CreatedBy = currentUser;
                        break;

                    case EntityState.Modified:
                        if (baseEntity.Status)
                        {
                            if (entry.Properties.Any(p => p.Metadata.Name == nameof(BaseEntity.CreatedBy)))
                            {
                                entry.Property(nameof(BaseEntity.CreatedBy)).IsModified = false;
                            }
                            if (entry.Properties.Any(p => p.Metadata.Name == nameof(BaseEntity.CreatedDate)))
                            {
                                entry.Property(nameof(BaseEntity.CreatedDate)).IsModified = false;
                            }
                            baseEntity.UpdatedDate = currentTime;
                            baseEntity.UpdatedBy = currentUser;
                        }
                        else
                        {
                            if (entry.Properties.Any(p => p.Metadata.Name == nameof(BaseEntity.CreatedBy)))
                            {
                                entry.Property(nameof(BaseEntity.CreatedBy)).IsModified = false;
                            }
                            if (entry.Properties.Any(p => p.Metadata.Name == nameof(BaseEntity.CreatedDate)))
                            {
                                entry.Property(nameof(BaseEntity.CreatedDate)).IsModified = false;
                            }
                            if (entry.Properties.Any(p => p.Metadata.Name == nameof(BaseEntity.UpdatedBy)))
                            {
                                entry.Property(nameof(BaseEntity.UpdatedBy)).IsModified = false;
                            }
                            if (entry.Properties.Any(p => p.Metadata.Name == nameof(BaseEntity.UpdatedDate)))
                            {
                                entry.Property(nameof(BaseEntity.UpdatedDate)).IsModified = false;
                            }
                            baseEntity.DeletedDate = currentTime;
                            baseEntity.DeletedBy = currentUser;
                        }
                        break;

                    case EntityState.Deleted:
                        break;
                }
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
    #endregion
}
