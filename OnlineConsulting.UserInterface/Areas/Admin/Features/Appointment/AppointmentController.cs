using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using OnlineConsulting.UserInterface.NotificationServices.ToastrServices;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Appointment;

[Area("Admin")]
[Route("admin/appointments")]
[Authorize(Policy = "RequireAdminAreaAccessPolicy")]
public class AppointmentController(IAppointmentDispatchService dispatchService, IToastNotification toastNotification) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index(string? status, CancellationToken cancellationToken)
    {
        ViewBag.SelectedStatus = status;
        return View(await dispatchService.GetAllAsync(status, cancellationToken));
    }

    [HttpPost("{id:guid}/confirm")]
    public async Task<IActionResult> Confirm(Guid id, CancellationToken cancellationToken)
    {
        var result = await dispatchService.ConfirmAsync(id, cancellationToken);
        toastNotification.ShowResult(result);

        return RedirectToAction("Index");
    }

    [HttpGet("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
    {
        var result = await dispatchService.CancelAsync(id, cancellationToken);
        toastNotification.ShowResult(result);

        return RedirectToAction("Index");
    }

    [HttpGet("{id:guid}/assign-technician")]
    public async Task<IActionResult> AssignTechnician(Guid id, CancellationToken cancellationToken) =>
        View(await dispatchService.GetAssignTechnicianFormAsync(id, cancellationToken));

    [HttpPost("{id:guid}/assign-technician")]
    public async Task<IActionResult> AssignTechnician(AssignTechnicianViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            model.Technicians = (await dispatchService.GetAssignTechnicianFormAsync(model.AppointmentId, cancellationToken)).Technicians;
            return View(model);
        }

        var result = await dispatchService.AssignTechnicianAsync(model, cancellationToken);
        toastNotification.ShowResult(result);

        return result.IsSuccessful ? RedirectToAction("Index") : View(model);
    }

    [HttpGet("{id:guid}/work-order")]
    public async Task<IActionResult> RecordWorkOrder(Guid id, CancellationToken cancellationToken)
    {
        var model = await dispatchService.GetRecordWorkOrderFormAsync(id, cancellationToken);
        return model is null ? NotFound() : View(model);
    }

    [HttpPost("{id:guid}/work-order")]
    public async Task<IActionResult> RecordWorkOrder(RecordWorkOrderViewModel recordViewModel, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            var refreshed = await dispatchService.GetRecordWorkOrderFormAsync(recordViewModel.AppointmentId, cancellationToken);
            recordViewModel.Technicians = refreshed?.Technicians ?? [];
            recordViewModel.ExistingEquipment = refreshed?.ExistingEquipment ?? [];
            return View(recordViewModel);
        }

        var result = await dispatchService.RecordWorkOrderAsync(recordViewModel, cancellationToken);
        toastNotification.ShowResult(result);

        if (result.IsSuccessful)
        {
            return RedirectToAction("Index");
        }

        var formOnFailure = await dispatchService.GetRecordWorkOrderFormAsync(recordViewModel.AppointmentId, cancellationToken);
        recordViewModel.Technicians = formOnFailure?.Technicians ?? [];
        recordViewModel.ExistingEquipment = formOnFailure?.ExistingEquipment ?? [];
        return View(recordViewModel);
    }

    [HttpGet("{id:guid}/work-order/view")]
    public async Task<IActionResult> WorkOrderDetail(Guid id, CancellationToken cancellationToken)
    {
        var model = await dispatchService.GetWorkOrderDetailAsync(id, cancellationToken);
        return model is null ? NotFound() : View(model);
    }
}
