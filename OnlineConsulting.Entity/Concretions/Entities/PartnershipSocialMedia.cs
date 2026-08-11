using OnlineConsulting.Entity.Concretions.BaseEntities;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineConsulting.Entity.Concretions.Entities;

public class PartnershipSocialMedia : BaseEntity
{
    public Guid ClassIconId { get; set; }
    public Guid PartnershipId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public Partnership Partnership { get; set; } = null!;
    public ClassIcon ClassIcon { get; set; } = null!;
    [NotMapped]
    public override string EntityName => "Partnership's Social Media Account";
}
