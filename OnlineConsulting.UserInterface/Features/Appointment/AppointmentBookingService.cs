using OnlineConsulting.UserInterface.Features.Service;
using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Features.Appointment;

public class AppointmentBookingService(IApiClient apiClient, IServiceCatalogService serviceCatalogService) : IAppointmentBookingService
{
    private const string AppointmentsPath = "/api/appointments";
    private const string AvailabilityPath = "/api/scheduling/availability";

    public async Task<List<ServiceOptionViewModel>> GetServiceOptionsAsync(CancellationToken cancellationToken = default)
    {
        var services = await serviceCatalogService.GetAllAsync(size: 100, cancellationToken: cancellationToken);
        return services.Select(s => new ServiceOptionViewModel(s.Id, s.Title)).ToList();
    }

    public async Task<List<AvailableSlotViewModel>> GetAvailableSlotsAsync(DateOnly date, CancellationToken cancellationToken = default)
    {
        var result = await apiClient.GetAsync<List<AvailableSlotViewModel>>($"{AvailabilityPath}?date={date:yyyy-MM-dd}", cancellationToken);
        return result.ResultData ?? [];
    }

    public Task<ApiEnvelope<Guid>> CreateAsync(BookAppointmentViewModel model, CancellationToken cancellationToken = default) =>
        apiClient.PostAsync<Guid>(AppointmentsPath, new
        {
            model.ServiceId,
            ScheduledStart = model.SelectedSlotStart,
            ScheduledEnd = model.SelectedSlotEnd,
            model.CustomerNote,
            model.ServiceAddress,
        }, cancellationToken);
}
