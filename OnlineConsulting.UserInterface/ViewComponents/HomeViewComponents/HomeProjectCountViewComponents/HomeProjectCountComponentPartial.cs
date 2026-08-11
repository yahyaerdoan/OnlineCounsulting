using Microsoft.AspNetCore.Mvc;

namespace OnlineConsulting.UserInterface.ViewComponents.HomeViewComponents.HomeProjectCountViewComponents;

public class HomeProjectCountComponentPartial : ViewComponent
{
    public IViewComponentResult Invoke() => View();
}
