using Microsoft.AspNetCore.Mvc;

namespace OnlineConsulting.UserInterface.ViewComponents.LayoutViewComponents.JavaScriptViewComponents;

public class LayoutJavaScriptViewComponentPartial : ViewComponent
{
    public IViewComponentResult Invoke() => View();
}
