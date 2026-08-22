using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using OnlineConsulting.UserInterface.NotificationServices.ToastrServices;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Equipment;

[Area("Admin")]
[Route("admin/equipment")]
[Authorize(Policy = "RequireAdminAreaAccessPolicy")]
public class EquipmentController(IEquipmentService equipmentService, IToastNotification toastNotification) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken) =>
        View(await equipmentService.GetAllAsync(cancellationToken));

    [HttpGet("create")]
    public async Task<IActionResult> Create(CancellationToken cancellationToken) =>
        View(new CreateEquipmentViewModel { Customers = await equipmentService.GetCustomerOptionsAsync(cancellationToken) });

    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateEquipmentViewModel createViewModel, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            createViewModel.Customers = await equipmentService.GetCustomerOptionsAsync(cancellationToken);
            return View(createViewModel);
        }

        var result = await equipmentService.CreateAsync(createViewModel, cancellationToken);
        toastNotification.ShowResult(result);

        if (!result.IsSuccessful)
        {
            createViewModel.Customers = await equipmentService.GetCustomerOptionsAsync(cancellationToken);
        }

        return result.IsSuccessful ? RedirectToAction("Index") : View(createViewModel);
    }

    [HttpGet("{id:guid}/edit")]
    public async Task<IActionResult> Update(Guid id, CancellationToken cancellationToken)
    {
        var model = await equipmentService.GetByIdAsync(id, cancellationToken);
        return model is null ? NotFound() : View(model);
    }

    [HttpPost("{id:guid}/edit")]
    public async Task<IActionResult> Update(UpdateEquipmentViewModel updateViewModel, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(updateViewModel);
        }

        var result = await equipmentService.UpdateAsync(updateViewModel, cancellationToken);
        toastNotification.ShowResult(result);

        return result.IsSuccessful ? RedirectToAction("Index") : View(updateViewModel);
    }

    [HttpGet("{id:guid}/delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await equipmentService.DeleteAsync(id, cancellationToken);
        toastNotification.ShowResult(result);

        return RedirectToAction("Index");
    }
}
