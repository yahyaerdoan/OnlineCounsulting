using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OnlineConsulting.UserInterface.Controllers;

[AllowAnonymous]
public class HomeController() : Controller
{
    [HttpGet]
    public IActionResult Index() => View();
}
