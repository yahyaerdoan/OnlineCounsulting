using OnlineConsulting.Modules.Scheduling.Domain;

namespace OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.Abstractions;

/// <summary>Tells everyone involved about an appointment's lifecycle step: the customer by email and push, the assigned technician and the
/// scheduling team by push (which also lands in their in-app inbox). Called after the change is saved; a delivery failure is logged, never
/// thrown, so it can't undo a change that already happened.</summary>
public interface IAppointmentNotifier
{
    Task RequestedAsync(Appointment appointment, CancellationToken cancellationToken = default);

    Task ConfirmedAsync(Appointment appointment, CancellationToken cancellationToken = default);

    Task CancelledByCustomerAsync(Appointment appointment, CancellationToken cancellationToken = default);

    Task CancelledByStaffAsync(Appointment appointment, string? reason, CancellationToken cancellationToken = default);

    Task TechnicianAssignedAsync(Appointment appointment, Guid? previousTechnicianUserId, CancellationToken cancellationToken = default);

    Task CompletedAsync(Appointment appointment, CancellationToken cancellationToken = default);
}
