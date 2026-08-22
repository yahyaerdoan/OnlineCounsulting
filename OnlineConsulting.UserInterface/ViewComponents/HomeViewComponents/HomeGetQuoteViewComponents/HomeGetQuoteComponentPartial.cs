using Microsoft.AspNetCore.Mvc;

namespace OnlineConsulting.UserInterface.ViewComponents.HomeViewComponents.HomeGetQuoteViewComponents;

public class HomeGetQuoteComponentPartial : ViewComponent
{
    public IViewComponentResult Invoke() => View();
}
