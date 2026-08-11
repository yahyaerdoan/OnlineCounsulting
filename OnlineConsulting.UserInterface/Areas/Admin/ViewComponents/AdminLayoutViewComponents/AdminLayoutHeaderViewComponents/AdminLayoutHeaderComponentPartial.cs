using Microsoft.AspNetCore.Mvc;

namespace OnlineConsulting.UserInterface.Areas.Admin.ViewComponents.AdminLayoutViewComponents.AdminLayoutHeaderViewComponents;

public class AdminLayoutHeaderComponentPartial : ViewComponent
{
    public IViewComponentResult Invoke() => View();
}
