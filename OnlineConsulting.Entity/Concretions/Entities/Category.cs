using OnlineConsulting.Entity.Concretions.BaseEntities;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineConsulting.Entity.Concretions.Entities;

public class Category : BaseEntity
{
    public Guid ImgIconId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<Service> Services { get; set; } = [];
    public ImgIcon ImgIcon { get; set; } = null!;
    [NotMapped]
    public override string EntityName => "Category";
}
