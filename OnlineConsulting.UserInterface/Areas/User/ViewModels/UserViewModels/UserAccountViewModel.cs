using OnlineConsulting.DataTransferObject.Concretions.Dtos.UserDtos;

namespace OnlineConsulting.UserInterface.Areas.User.ViewModels.UserViewModels;

public class UserAccountViewModel
{
    public required ResultUserDto User { get; set; }
    public ChangePasswordUserDto ChangePassword { get; set; } = new();
}
