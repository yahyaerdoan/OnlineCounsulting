namespace OnlineConsulting.UserInterface.Areas.User.Features.Appointment;

/// <summary>The dashboard's "My Appointments" screen - resolves ServiceId to a display name in bulk
/// (IServiceCatalogService.GetAllAsync once) rather than one Api call per appointment.</summary>
public interface IUserAppointmentPageService
{
    Task<List<UserAppointmentListItemViewModel>> GetMyAppointmentsAsync(CancellationToken cancellationToken = default);
}
