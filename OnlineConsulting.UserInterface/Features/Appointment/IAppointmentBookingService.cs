using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Features.Appointment;

/// <summary>All Api orchestration for the public appointment-booking flow - AppointmentController only calls
/// this and renders the result, it never talks to IApiClient/IServiceCatalogService directly.</summary>
public interface IAppointmentBookingService
{
    Task<List<ServiceOptionViewModel>> GetServiceOptionsAsync(CancellationToken cancellationToken = default);
    Task<List<AvailableSlotViewModel>> GetAvailableSlotsAsync(DateOnly date, CancellationToken cancellationToken = default);
    Task<ApiEnvelope<Guid>> CreateAsync(BookAppointmentViewModel model, CancellationToken cancellationToken = default);
}
