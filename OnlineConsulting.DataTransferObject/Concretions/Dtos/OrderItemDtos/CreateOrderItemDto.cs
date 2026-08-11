using OnlineConsulting.DataTransferObject.Abstractions.IDtos;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.OrderItemDtos;

public class CreateOrderItemDto : IDto
{
    public Guid OrderId { get; set; }
    public Guid ServiceId { get; set; }

    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public float TaxRate { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal SubTotalPrice { get; set; }
    public decimal TotalPrice { get; set; }
}
