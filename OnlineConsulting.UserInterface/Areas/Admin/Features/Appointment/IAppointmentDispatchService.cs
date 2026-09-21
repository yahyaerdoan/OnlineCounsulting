using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Appointment;

/// <summary>Admin appointment-dispatch orchestration; names are resolved via bulk lookups since AppointmentResponse only carries raw ids.</summary>
public interface IAppointmentDispatchService
{
    /// <summary>Lists appointments, optionally filtered by status, with customer/service/technician names resolved.</summary>
    Task<List<AppointmentListItemViewModel>> GetAllAsync(string? status, CancellationToken cancellationToken = default);

    Task<ApiEnvelope> ConfirmAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> CancelAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Builds the technician-assignment form, pre-loaded with the available technician options.</summary>
    Task<AssignTechnicianViewModel> GetAssignTechnicianFormAsync(Guid appointmentId, CancellationToken cancellationToken = default);

    Task<ApiEnvelope> AssignTechnicianAsync(AssignTechnicianViewModel model, CancellationToken cancellationToken = default);

    /// <summary>Builds the work-order form for an appointment, including its technicians and the customer's existing equipment. Null if the appointment doesn't exist.</summary>
    Task<RecordWorkOrderViewModel?> GetRecordWorkOrderFormAsync(Guid appointmentId, CancellationToken cancellationToken = default);

    /// <summary>Records a work order, creating new equipment inline if none was selected, and uploads before/after photos.</summary>
    Task<ApiEnvelope> RecordWorkOrderAsync(RecordWorkOrderViewModel model, CancellationToken cancellationToken = default);

    /// <summary>Gets the completed work order for an appointment for read-only display. Null if none exists.</summary>
    Task<WorkOrderDetailViewModel?> GetWorkOrderDetailAsync(Guid appointmentId, CancellationToken cancellationToken = default);
}
