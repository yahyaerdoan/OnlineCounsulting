using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OnlineConsulting.UserInterface.Features.Home;

/// <summary>Public home page - renders a static shell, every widget on it is its own ViewComponent
/// (HomeCategoriesComponentPartial/HomeOurServicesComponentPartial etc.) so there's no data to fetch here.</summary>
[AllowAnonymous]
public class HomeController : Controller
{
    [HttpGet]
    public IActionResult Index() => View();
}
