using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using OnlineConsulting.UserInterface.Infrastructure.Api;
using OnlineConsulting.UserInterface.NotificationServices.ToastrServices;

namespace OnlineConsulting.UserInterface.Features.Appointment;

/// <summary>Requires login like Checkout does (no [AllowAnonymous], relies on the app-wide
/// RequireAuthenticatedUser default policy in Program.cs) - CreateAppointment needs a known user/email anyway.</summary>
[Route("appointments")]
public class AppointmentController(IAppointmentBookingService bookingService, IToastNotification toastNotification) : Controller
{
    [HttpGet("book")]
    public async Task<IActionResult> Book(Guid? serviceId, DateOnly? date, CancellationToken cancellationToken)
    {
        var model = new BookAppointmentViewModel
        {
            ServiceId = serviceId,
            Date = date ?? DateOnly.FromDateTime(DateTime.UtcNow),
            Services = await bookingService.GetServiceOptionsAsync(cancellationToken),
        };

        if (date is not null)
        {
            model.AvailableSlots = await bookingService.GetAvailableSlotsAsync(date.Value, cancellationToken);
        }

        return View(model);
    }

    [HttpPost("book")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Book(BookAppointmentViewModel model, CancellationToken cancellationToken)
    {
        model.Services = await bookingService.GetServiceOptionsAsync(cancellationToken);
        model.AvailableSlots = await bookingService.GetAvailableSlotsAsync(model.Date, cancellationToken);

        if (model.SelectedSlotStart is null || model.SelectedSlotEnd is null)
        {
            ModelState.AddModelError(string.Empty, "Please select an available time slot.");
            return View(model);
        }

        var result = await bookingService.CreateAsync(model, cancellationToken);
        toastNotification.ShowResult(result.WithoutData());

        return result.IsSuccessful
            ? RedirectToAction("Appointment", "Dashboard", new { area = "User" })
            : View(model);
    }
}
