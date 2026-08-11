using OnlineConsulting.Entity.Concretions.BaseEntities;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineConsulting.Entity.Concretions.Entities;

public class Message : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    [NotMapped]
    public override string? CreatedBy { get => base.CreatedBy; set => base.CreatedBy = value; }
    [NotMapped]
    public override string? UpdatedBy { get => base.UpdatedBy; set => base.UpdatedBy = value; }
    [NotMapped]
    public override DateTime? UpdatedDate { get => base.UpdatedDate; set => base.UpdatedDate = value; }
    [NotMapped]
    public override string EntityName => "Message";
}
