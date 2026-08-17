using Microsoft.AspNetCore.Mvc;

namespace OnlineConsulting.UserInterface.Areas.User.Features.UserAddress;

/// <summary>The dashboard's "My Addresses" tab - lives in the UserAddress feature folder with the controller
/// that its edit/delete links post to. The view stays at Areas/User/Views/Shared/Components/{ClassName}/Default.cshtml
/// (Razor's fixed lookup for view components).</summary>
public class DashboardAddressListComponentPartial(IUserAddressPageService userAddressPageService) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync() =>
        View(await userAddressPageService.GetListAsync());
}
