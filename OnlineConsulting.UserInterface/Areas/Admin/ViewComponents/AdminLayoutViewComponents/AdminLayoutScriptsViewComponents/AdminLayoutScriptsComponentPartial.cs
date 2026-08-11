using Microsoft.AspNetCore.Mvc;

namespace OnlineConsulting.UserInterface.Areas.Admin.ViewComponents.AdminLayoutViewComponents.AdminLayoutScriptsViewComponents;

public class AdminLayoutScriptsComponentPartial : ViewComponent
{
    public IViewComponentResult Invoke() => View();
}
