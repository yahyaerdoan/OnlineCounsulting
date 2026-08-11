using OnlineConsulting.Entity.Concretions.BaseEntities;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineConsulting.Entity.Concretions.Entities;

public class HowIGetService : BaseEntity
{
    public Guid ImgIconId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ImgIcon ImgIcon { get; set; } = null!;
    [NotMapped]
    public override string EntityName => "How I Get Service Stage";
}
