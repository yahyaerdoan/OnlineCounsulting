using Microsoft.AspNetCore.Mvc;

namespace OnlineConsulting.UserInterface.Areas.User.Features.UserAddress;

/// <summary>The dashboard's "Add New Address" tab - no data of its own, it only renders the empty create form
/// that UserAddressController's create action receives.</summary>
public class DashboardAddressCreateComponentPartial : ViewComponent
{
    public IViewComponentResult Invoke() => View(new CreateUserAddressViewModel());
}
