using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using OnlineConsulting.UserInterface.NotificationServices.ToastrServices;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.HowIGetService;

[Area("Admin")]
[Route("admin/how-i-get-service")]
[Authorize(Policy = "RequireAdminAreaAccessPolicy")]
public class HowIGetServiceController(IHowIGetServiceService howIGetServiceService, IToastNotification toastNotification) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken) =>
        View(await howIGetServiceService.GetAllAsync(cancellationToken));

    [HttpGet("create")]
    public IActionResult Create() => View(new CreateHowIGetServiceViewModel());

    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateHowIGetServiceViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await howIGetServiceService.CreateAsync(model, cancellationToken);
        toastNotification.ShowResult(result);

        return result.IsSuccessful ? RedirectToAction("Index") : View(model);
    }

    [HttpGet("{id:guid}/edit")]
    public async Task<IActionResult> Update(Guid id, CancellationToken cancellationToken)
    {
        var model = await howIGetServiceService.GetByIdAsync(id, cancellationToken);
        return model is null ? NotFound() : View(model);
    }

    [HttpPost("{id:guid}/edit")]
    public async Task<IActionResult> Update(UpdateHowIGetServiceViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await howIGetServiceService.UpdateAsync(model, cancellationToken);
        toastNotification.ShowResult(result);

        return result.IsSuccessful ? RedirectToAction("Index") : View(model);
    }

    [HttpGet("{id:guid}/delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await howIGetServiceService.DeleteAsync(id, cancellationToken);
        toastNotification.ShowResult(result);

        return RedirectToAction("Index");
    }
}
