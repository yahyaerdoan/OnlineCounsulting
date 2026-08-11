using OnlineConsulting.Entity.Concretions.BaseEntities;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineConsulting.Entity.Concretions.Entities;


public class GalleryCategory : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    // Navigation property for many-to-many relationship
    public ICollection<GalleryItemCategory> GalleryCategories { get; set; } = [];

    [NotMapped]
    public override string EntityName => "Gallery Category";
}
