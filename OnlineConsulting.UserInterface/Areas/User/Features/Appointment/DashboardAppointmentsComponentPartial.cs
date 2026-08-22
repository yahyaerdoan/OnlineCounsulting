using Microsoft.AspNetCore.Mvc;

namespace OnlineConsulting.UserInterface.Areas.User.Features.Appointment;

public class DashboardAppointmentsComponentPartial(IUserAppointmentPageService pageService) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync() =>
        View(await pageService.GetMyAppointmentsAsync());
}
