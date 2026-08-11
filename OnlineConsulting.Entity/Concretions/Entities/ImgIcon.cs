using OnlineConsulting.Entity.Concretions.BaseEntities;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineConsulting.Entity.Concretions.Entities;

public class ImgIcon : BaseEntity
{
    public string IconUrl { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ICollection<Category> Categories { get; set; } = [];
    public ICollection<HowIGetService> HowIGetServices { get; set; } = [];
    public ICollection<ProvidedItem> ProvidedItems { get; set; } = [];
    [NotMapped]
    public override string EntityName => "Img Icon";
}
