using OnlineConsulting.Entity.Concretions.BaseEntities;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineConsulting.Entity.Concretions.Entities;

public class OrderItem : BaseEntity
{
    public Guid OrderId { get; set; }
    public Guid ServiceId { get; set; }

    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public float TaxRate { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal SubTotalPrice { get; set; }
    public decimal TotalPrice { get; set; }

    public Order Order { get; set; } = null!;
    public Service Service { get; set; } = null!;

    [NotMapped]
    public override string EntityName => "Order Item";
}
