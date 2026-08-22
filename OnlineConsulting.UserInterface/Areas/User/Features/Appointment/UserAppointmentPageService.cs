using OnlineConsulting.UserInterface.Features.Service;

namespace OnlineConsulting.UserInterface.Areas.User.Features.Appointment;

public class UserAppointmentPageService(IAppointmentService appointmentService, IServiceCatalogService serviceCatalogService) : IUserAppointmentPageService
{
    public async Task<List<UserAppointmentListItemViewModel>> GetMyAppointmentsAsync(CancellationToken cancellationToken = default)
    {
        var appointmentsTask = appointmentService.GetMineAsync(cancellationToken);
        var servicesTask = serviceCatalogService.GetAllAsync(size: 100, cancellationToken: cancellationToken);
        await Task.WhenAll(appointmentsTask, servicesTask);

        var serviceNamesById = servicesTask.Result.ToDictionary(s => s.Id, s => s.Title);

        return appointmentsTask.Result
            .OrderByDescending(a => a.ScheduledStart)
            .Select(a => new UserAppointmentListItemViewModel(
                a.Id,
                a.ServiceId is not null && serviceNamesById.TryGetValue(a.ServiceId.Value, out var name) ? name : "General meeting",
                a.ScheduledStart,
                a.ScheduledEnd,
                a.Status,
                a.CustomerNote,
                a.ServiceAddress,
                a.NavigationUrl,
                a.Status is "Pending" or "Confirmed"))
            .ToList();
    }
}
