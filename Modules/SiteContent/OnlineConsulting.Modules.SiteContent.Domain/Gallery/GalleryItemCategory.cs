using Core.PersistenceLayer.MultiTenancy;

namespace OnlineConsulting.Modules.SiteContent.Domain.Gallery;

/// <summary>Links a <see cref="GalleryItem"/> to one GalleryCategory; created and removed only by the item.</summary>
public class GalleryItemCategory : TenantEntity<Guid>
{
    private GalleryItemCategory()
    {
    }

    public Guid GalleryItemId { get; private set; }

    public Guid GalleryCategoryId { get; private set; }

    internal static GalleryItemCategory Create(Guid galleryItemId, Guid galleryCategoryId) =>
        new() { GalleryItemId = galleryItemId, GalleryCategoryId = galleryCategoryId };
}
