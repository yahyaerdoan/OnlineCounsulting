using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OnlineConsulting.UserInterface.Controllers;

[AllowAnonymous]
public class ErrorPageController : Controller
{
    [HttpGet]
    public new IActionResult Unauthorized() => View();
    [HttpGet]
    public new IActionResult NotFound() => View();
    [HttpGet]
    public IActionResult NotAllowed() => View();
    [HttpGet]
    public IActionResult Forbidden() => View();
    [HttpGet]
    public IActionResult InternalServerError() => View();

    //Target of app.UseStatusCodePagesWithReExecute("/errorpage/{0}")
    [HttpGet("errorpage/{statusCode:int}")]
    public IActionResult ByStatusCode(int statusCode) => statusCode switch
    {
        401 => View("Unauthorized"),
        403 => View("Forbidden"),
        404 => View("NotFound"),
        405 => View("NotAllowed"),
        _ => View("InternalServerError")
    };
}
