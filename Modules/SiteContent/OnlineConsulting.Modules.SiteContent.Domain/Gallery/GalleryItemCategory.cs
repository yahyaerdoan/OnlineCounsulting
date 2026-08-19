using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.SiteContent.Domain.Gallery;

/// <summary>Explicit many-to-many join between GalleryItem and GalleryCategory - a GalleryItem can carry multiple tags, matching the legacy "at least one category required" rule enforced by the Create/UpdateGalleryItem validators.</summary>
public class GalleryItemCategory : TenantEntity<Guid>
{
    public required Guid GalleryItemId { get; set; }
    public required Guid GalleryCategoryId { get; set; }
}
