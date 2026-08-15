using OnlineConsulting.Entity.Concretions.BaseEntities;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineConsulting.Entity.Concretions.Entities;


public class GalleryCategory : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    /// <summary>Navigation property for the many-to-many relationship with gallery items.</summary>
    public ICollection<GalleryItemCategory> GalleryCategories { get; set; } = [];

    [NotMapped]
    public override string EntityName => "Gallery Category";
}
