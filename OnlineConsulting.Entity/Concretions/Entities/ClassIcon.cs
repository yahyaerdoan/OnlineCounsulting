using OnlineConsulting.Entity.Concretions.BaseEntities;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineConsulting.Entity.Concretions.Entities;

public class ClassIcon : BaseEntity
{
    public string IconClass { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ICollection<PartnershipSocialMedia> PartnershipSocialMedias { get; set; } = [];
    public ICollection<SocialMedia> SocialMedias { get; set; } = [];
    [NotMapped]
    public override string EntityName => "Class Icon";
}
