using Microsoft.AspNetCore.Mvc;

namespace OnlineConsulting.UserInterface.Areas.Admin.ViewComponents.AdminLayoutViewComponents.AdminLayoutFooterViewComponents;

public class AdminLayoutFooterComponentPartial : ViewComponent
{
    public IViewComponentResult Invoke() => View();
}
