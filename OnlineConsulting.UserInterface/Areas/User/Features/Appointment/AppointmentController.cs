using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using OnlineConsulting.UserInterface.NotificationServices.ToastrServices;

namespace OnlineConsulting.UserInterface.Areas.User.Features.Appointment;

[Area("User")]
[Route("User/{controller}/{action}/{id?}")]
public class AppointmentController(IAppointmentService appointmentService, IToastNotification toastNotification) : Controller
{
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
    {
        var result = await appointmentService.CancelAsync(id, cancellationToken);
        toastNotification.ShowResult(result);

        return RedirectToAction("Appointment", "Dashboard", new { area = "User" });
    }
}
