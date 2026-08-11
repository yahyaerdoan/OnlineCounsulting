using OnlineConsulting.DataTransferObject.Abstractions.IDtos;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.UserAddressDtos;

public class SetShippingAddressDto : IDto
{
    public string AddressId { get; set; } = string.Empty;
    public string OldAddressId { get; set; } = string.Empty;
    public string? CartId { get; set; }
}
