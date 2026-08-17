using System.ComponentModel.DataAnnotations;

namespace OnlineConsulting.UserInterface.Areas.User.Features.UserAddress;

public record UserAddressListItemViewModel(
    Guid Id,
    string AddressName,
    string? CompanyName,
    string Country,
    string AddressLine,
    string City,
    string State,
    string Zipcode,
    string? Notes,
    bool IsShippingAddress,
    bool IsBillingAddress);

public class AddressListPageViewModel
{
    public List<UserAddressListItemViewModel> UserAddressList { get; set; } = [];
}

public class CreateUserAddressViewModel
{
    [Required]
    public string AddressName { get; set; } = string.Empty;

    public string? CompanyName { get; set; }

    [Required]
    public string Country { get; set; } = string.Empty;

    [Required]
    public string AddressLine { get; set; } = string.Empty;

    [Required]
    public string City { get; set; } = string.Empty;

    [Required]
    public string State { get; set; } = string.Empty;

    [Required]
    public string Zipcode { get; set; } = string.Empty;

    public string? Notes { get; set; }

    public bool IsShippingAddress { get; set; }

    public bool IsBillingAddress { get; set; }
}

public class UpdateUserAddressViewModel : CreateUserAddressViewModel
{
    public Guid Id { get; set; }
}
