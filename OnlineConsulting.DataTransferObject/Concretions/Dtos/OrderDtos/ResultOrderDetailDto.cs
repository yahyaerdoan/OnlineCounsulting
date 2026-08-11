using OnlineConsulting.DataTransferObject.Concretions.Dtos.OrderItemDtos;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.ServiceDtos;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.UserAddressDtos;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.OrderDtos;

public class ResultOrderDetailDto
{
    public ResultOrderDto Order { get; set; } = null!;
    public List<ResultOrderItemDto> OrderItems { get; set; } = [];
    public List<ResultServiceWithImageDto> Services { get; set; } = [];
    public ResultUserAddressDto ShippingAddress { get; set; } = null!;
    public ResultUserAddressDto InvoiceAddress { get; set; } = null!;
}
