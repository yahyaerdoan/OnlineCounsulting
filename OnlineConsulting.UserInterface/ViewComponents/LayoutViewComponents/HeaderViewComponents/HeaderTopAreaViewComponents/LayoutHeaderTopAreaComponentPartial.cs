using Microsoft.AspNetCore.Mvc;

namespace OnlineConsulting.UserInterface.ViewComponents.LayoutViewComponents.HeaderViewComponents.HeaderTopAreaViewComponents;

public class LayoutHeaderTopAreaComponentPartial : ViewComponent
{
    public IViewComponentResult Invoke() => View();
}
