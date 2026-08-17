using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using OnlineConsulting.UserInterface.NotificationServices.ToastrServices;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.AboutUs;

[Area("Admin")]
[Route("admin/about-us")]
[Authorize(Policy = "RequireAdminAreaAccessPolicy")]
public class AboutUsController(IAboutUsService aboutUsService, IToastNotification toastNotification) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken) =>
        View(await aboutUsService.GetAllAsync(cancellationToken));

    [HttpGet("create")]
    public IActionResult Create() => View(new CreateAboutUsViewModel());

    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateAboutUsViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await aboutUsService.CreateAsync(model, cancellationToken);
        toastNotification.ShowResult(result);

        return result.IsSuccessful ? RedirectToAction("Index") : View(model);
    }

    [HttpGet("{id:guid}/edit")]
    public async Task<IActionResult> Update(Guid id, CancellationToken cancellationToken)
    {
        var model = await aboutUsService.GetByIdAsync(id, cancellationToken);
        return model is null ? NotFound() : View(model);
    }

    [HttpPost("{id:guid}/edit")]
    public async Task<IActionResult> Update(UpdateAboutUsViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await aboutUsService.UpdateAsync(model, cancellationToken);
        toastNotification.ShowResult(result);

        return result.IsSuccessful ? RedirectToAction("Index") : View(model);
    }

    [HttpGet("{id:guid}/delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await aboutUsService.DeleteAsync(id, cancellationToken);
        toastNotification.ShowResult(result);

        return RedirectToAction("Index");
    }
}
