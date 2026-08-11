using Microsoft.AspNetCore.Mvc;

namespace OnlineConsulting.UserInterface.Areas.User.ViewComponents.DashboardViewComponents.DashboardSidebarViewComponents;

public class DashboardSidebarComponentPartial : ViewComponent
{
    public IViewComponentResult Invoke() => View();
}
