using OnlineConsulting.Entity.Concretions.BaseEntities;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineConsulting.Entity.Concretions.Entities;

public class UserAddress : BaseEntity
{
    public string AddressName { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    public string Country { get; set; } = string.Empty;
    public string AddressLine { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Zipcode { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public bool IsShippingAddress { get; set; }
    public bool IsBillingAddress { get; set; }
    public Guid UserId { get; set; }

    // No User navigation: User lives in a separate module/DbContext (Auth).
    public ICollection<Order> OrderInvoiceAddress { get; set; } = [];
    public ICollection<Order> OrderShippingAddress { get; set; } = [];

    [NotMapped]
    public override string EntityName => "User Address";
}
