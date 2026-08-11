using OnlineConsulting.DataTransferObject.Abstractions.IDtos;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.OrderDtos;

public class CreateOrderDto : IDto
{
    public string OrderNumber { get; set; } = string.Empty;
    public string OrderStatus { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;
    public Guid ShippingAddressId { get; set; }
    public Guid InvoiceAddressId { get; set; }
}
