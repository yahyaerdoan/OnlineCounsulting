using OnlineConsulting.Entity.Concretions.BaseEntities;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineConsulting.Entity.Concretions.Entities;

public class GalleryItem : BaseEntity
{
    public string ImageUrl { get; set; } = string.Empty;
    public string? Description { get; set; }

    /// <summary>Navigation property for the many-to-many relationship with gallery categories.</summary>
    public ICollection<GalleryItemCategory> GalleryCategories { get; set; } = [];
    [NotMapped]
    public List<string> GalleryCategoryIds { get; set; } = [];

    [NotMapped]
    public override string EntityName => "Gallery item";
}
