namespace OnlineConsulting.Entity.Concretions.Entities;

public class GalleryItemCategory
{
    public Guid GalleryCategoriesId { get; set; }
    public GalleryCategory GalleryCategory { get; set; } = null!;
    public Guid GalleryItemsId { get; set; }
    public GalleryItem GalleryItem { get; set; } = null!;
}
