using Microsoft.AspNetCore.Mvc;

namespace OnlineConsulting.UserInterface.Areas.User.ViewComponents.DashboardViewComponents.DashboardAddressViewComponents;

public class DashboardAddressCreateComponentPartial : ViewComponent
{
    public IViewComponentResult Invoke() =>
        //ViewBag.AnyShippingAddress = serviceManager.UserAddressService.UserAnyShippingAddress();
        //ViewBag.AnyBillingAddress = serviceManager.UserAddressService.UserAnyBillingAddress();
        View();
}
