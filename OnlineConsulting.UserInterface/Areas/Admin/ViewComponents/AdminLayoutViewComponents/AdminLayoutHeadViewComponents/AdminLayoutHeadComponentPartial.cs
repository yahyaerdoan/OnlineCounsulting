using Microsoft.AspNetCore.Mvc;

namespace OnlineConsulting.UserInterface.Areas.Admin.ViewComponents.AdminLayoutViewComponents.AdminLayoutHeadViewComponents;

public class AdminLayoutHeadComponentPartial : ViewComponent
{
    public IViewComponentResult Invoke() => View();
}
