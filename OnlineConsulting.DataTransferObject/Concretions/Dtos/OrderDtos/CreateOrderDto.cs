using OnlineConsulting.DataTransferObject.Abstractions.IDtos;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.OrderDtos;

public class CreateOrderDto : IDto
{
    public string OrderNumber { get; set; } = string.Empty;
    public string OrderStatus { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;

    public Guid UserId { get; set; }
    public Guid ShippingAddressId { get; set; }
    public Guid InvoiceAddressId { get; set; }
}
