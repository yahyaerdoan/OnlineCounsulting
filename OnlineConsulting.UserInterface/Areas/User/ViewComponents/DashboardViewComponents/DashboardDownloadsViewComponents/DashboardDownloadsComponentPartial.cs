using Microsoft.AspNetCore.Mvc;

namespace OnlineConsulting.UserInterface.Areas.User.ViewComponents.DashboardViewComponents.DashboardDownloadsViewComponents;

public class DashboardDownloadsComponentPartial : ViewComponent
{
    public IViewComponentResult Invoke() => View();
}
