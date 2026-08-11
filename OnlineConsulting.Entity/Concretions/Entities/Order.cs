using OnlineConsulting.Entity.Concretions.BaseEntities;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineConsulting.Entity.Concretions.Entities;

public class Order : BaseEntity
{
    public string OrderNumber { get; set; } = string.Empty;
    public string OrderStatus { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;
    public Guid ShippingAddressId { get; set; }
    public Guid InvoiceAddressId { get; set; }

    public UserAddress ShippingAddress { get; set; } = null!;
    public UserAddress InvoiceAddress { get; set; } = null!;
    public User User { get; set; } = null!;

    public ICollection<OrderItem> OrderItems { get; set; } = [];

    [NotMapped]
    public override string EntityName => "Order";
}
