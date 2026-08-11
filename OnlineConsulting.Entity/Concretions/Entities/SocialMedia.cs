using OnlineConsulting.Entity.Concretions.BaseEntities;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineConsulting.Entity.Concretions.Entities;

public class SocialMedia : BaseEntity
{
    public Guid ClassIconId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public ClassIcon ClassIcon { get; set; } = null!;
    [NotMapped]
    public override string EntityName => "Social Media Account";
}
