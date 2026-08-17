using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OnlineConsulting.UserInterface.Features.Partnership;

[AllowAnonymous]
public class PartnershipController : Controller
{
    [HttpGet]
    public IActionResult Index() => View();
}
