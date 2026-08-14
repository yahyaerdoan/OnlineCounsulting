using OnlineConsulting.DataTransferObject.Abstractions.IDtos;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.UserAddressDtos;

public class CreateUserAddressDto : IDto
{
    public Guid? UserId { get; set; }
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
}
