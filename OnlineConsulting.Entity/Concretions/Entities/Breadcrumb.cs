using OnlineConsulting.Entity.Concretions.BaseEntities;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineConsulting.Entity.Concretions.Entities;

public class Breadcrumb : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    [NotMapped]
    public override string? DeletedBy { get => base.DeletedBy; set => base.DeletedBy = value; }
    [NotMapped]
    public override DateTime? DeletedDate { get => base.DeletedDate; set => base.DeletedDate = value; }
    [NotMapped]
    public override string EntityName => "Breadcrumb";
}
