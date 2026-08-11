using Microsoft.AspNetCore.Mvc;

namespace OnlineConsulting.UserInterface.Areas.Admin.ViewComponents.AdminLayoutViewComponents.AdminLayoutBreadcrumbViewComponents;

public class AdminLayoutBreadcrumbComponentPartial : ViewComponent
{
    public IViewComponentResult Invoke() => View();
}
