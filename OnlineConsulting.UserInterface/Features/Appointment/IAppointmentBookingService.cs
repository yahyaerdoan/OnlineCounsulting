using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Features.Appointment;

/// <summary>All Api orchestration for the public appointment-booking flow - AppointmentController only calls
/// this and renders the result, it never talks to IApiClient/IServiceCatalogService directly.</summary>
public interface IAppointmentBookingService
{
    /// <summary>Gets bookable services for the appointment selector.</summary>
    Task<List<ServiceOptionViewModel>> GetServiceOptionsAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets available scheduling slots for the given date.</summary>
    Task<List<AvailableSlotViewModel>> GetAvailableSlotsAsync(DateOnly date, CancellationToken cancellationToken = default);

    /// <summary>Creates an appointment from the booking form.</summary>
    /// <returns>Envelope carrying the new appointment id.</returns>
    Task<ApiEnvelope<Guid>> CreateAsync(BookAppointmentViewModel model, CancellationToken cancellationToken = default);
}
