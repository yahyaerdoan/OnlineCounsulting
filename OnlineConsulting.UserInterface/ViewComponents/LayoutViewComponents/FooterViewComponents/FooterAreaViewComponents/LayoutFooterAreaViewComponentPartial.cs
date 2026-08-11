using Microsoft.AspNetCore.Mvc;

namespace OnlineConsulting.UserInterface.ViewComponents.LayoutViewComponents.FooterViewComponents.FooterAreaViewComponents;

public class LayoutFooterAreaViewComponentPartial : ViewComponent
{
    public IViewComponentResult Invoke() => View();
}
