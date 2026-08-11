using Microsoft.AspNetCore.Mvc;

namespace OnlineConsulting.UserInterface.ViewComponents.LayoutViewComponents.HeadViewComponents;

public class LayoutHeadViewComponentPartial : ViewComponent
{
    public IViewComponentResult Invoke() => View();
}
