using OnlineConsulting.Entity.Concretions.BaseEntities;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineConsulting.Entity.Concretions.Entities;

public class ServiceImage : BaseEntity
{
    public Guid ServiceId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public Service Service { get; set; } = null!;
    [NotMapped]
    public override string EntityName => "Service Image";
}
