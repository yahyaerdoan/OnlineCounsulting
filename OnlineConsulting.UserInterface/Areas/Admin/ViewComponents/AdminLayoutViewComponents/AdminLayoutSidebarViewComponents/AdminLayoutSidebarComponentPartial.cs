using Microsoft.AspNetCore.Mvc;

namespace OnlineConsulting.UserInterface.Areas.Admin.ViewComponents.AdminLayoutViewComponents.AdminLayoutSidebarViewComponents;

public class AdminLayoutSidebarComponentPartial : ViewComponent
{
    public IViewComponentResult Invoke() => View();
}
