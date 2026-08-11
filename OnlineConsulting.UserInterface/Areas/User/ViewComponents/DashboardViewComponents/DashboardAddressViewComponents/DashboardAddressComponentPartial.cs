using Microsoft.AspNetCore.Mvc;

namespace OnlineConsulting.UserInterface.Areas.User.ViewComponents.DashboardViewComponents.DashboardAddressViewComponents;

public class DashboardAddressComponentPartial : ViewComponent
{
    public IViewComponentResult Invoke() => View();
}
