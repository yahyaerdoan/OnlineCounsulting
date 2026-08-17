using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OnlineConsulting.UserInterface.Features.Gallery;

/// <summary>Public gallery page - renders a static shell, GalleryCategoriesComponentPartial/
/// GalleryImagesComponentPartial fetch their own data.</summary>
[AllowAnonymous]
public class GalleryController : Controller
{
    [HttpGet]
    public IActionResult Index() => View();
}
