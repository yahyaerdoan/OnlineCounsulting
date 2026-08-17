using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OnlineConsulting.UserInterface.Features.AboutUs;

/// <summary>Public About Us page - unchanged from the legacy controller, it renders a static view with no
/// model, so there's no Api call to migrate here.</summary>
[AllowAnonymous]
public class AboutUsController : Controller
{
    [HttpGet]
    public IActionResult Index() => View();
}
