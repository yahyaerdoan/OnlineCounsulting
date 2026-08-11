using OnlineConsulting.DataTransferObject.Concretions.Dtos.UserAddressDtos;

namespace OnlineConsulting.UserInterface.Areas.User.ViewModels.AddressViewModels;

public class AddressListPageViewModel
{
    public required IQueryable<ResultUserAddressDto> UserAddressList { get; set; }
    public UpdateUserAddressDto UpdateUserAddressDto { get; set; } = new();
}
