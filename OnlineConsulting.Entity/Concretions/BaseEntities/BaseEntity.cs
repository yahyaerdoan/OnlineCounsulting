using OnlineConsulting.Entity.Abstractions;

namespace OnlineConsulting.Entity.Concretions.BaseEntities;

public class BaseEntity : IEntity
{
    public Guid Id { get; set; }
    public DateTime? CreatedDate { get; set; }
    public virtual DateTime? UpdatedDate { get; set; }
    public virtual string? CreatedBy { get; set; }
    public virtual string? UpdatedBy { get; set; }
    public virtual string? DeletedBy { get; set; }
    public virtual DateTime? DeletedDate { get; set; }
    public bool Status { get; set; }
    public virtual string EntityName => GetType().Name;
}
