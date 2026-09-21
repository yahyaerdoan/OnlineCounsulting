using Microsoft.AspNetCore.Mvc;

namespace OnlineConsulting.UserInterface.Areas.User.Features.UserAddress;

/// <summary>The dashboard's "My Addresses" tab; view stays at the fixed Razor view-component lookup path.</summary>
public class DashboardAddressListComponentPartial(IUserAddressPageService userAddressPageService) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync() =>
        View(await userAddressPageService.GetListAsync());
}
