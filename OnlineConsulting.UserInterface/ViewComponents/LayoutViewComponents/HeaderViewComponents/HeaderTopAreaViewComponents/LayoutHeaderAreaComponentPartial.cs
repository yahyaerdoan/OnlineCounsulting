using Microsoft.AspNetCore.Mvc;

namespace OnlineConsulting.UserInterface.ViewComponents.LayoutViewComponents.HeaderViewComponents.HeaderTopAreaViewComponents;

public class LayoutHeaderAreaComponentPartial : ViewComponent
{
    public IViewComponentResult Invoke() => View();
}
