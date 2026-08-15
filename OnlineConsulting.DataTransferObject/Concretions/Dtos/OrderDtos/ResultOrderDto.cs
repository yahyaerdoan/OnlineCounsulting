using OnlineConsulting.DataTransferObject.Abstractions.IDtos;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.OrderDtos;

public class ResultOrderDto : IDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    /// <summary>Populated by OrderManager via a lookup against the Identity module since Order and User live in separate DbContexts.</summary>
    public string UserFirstName { get; set; } = string.Empty;
    public string UserLastName { get; set; } = string.Empty;
    public string OrderNumber { get; set; } = string.Empty;
    public string OrderStatus { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public Guid ShippingAddressId { get; set; }
    public Guid InvoiceAddressId { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public string? DeletedBy { get; set; }
    public DateTime? DeletedDate { get; set; }
}
