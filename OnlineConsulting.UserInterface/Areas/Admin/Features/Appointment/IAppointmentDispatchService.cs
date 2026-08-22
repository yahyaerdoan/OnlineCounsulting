using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Appointment;

/// <summary>All Api orchestration for the admin appointment-dispatch screen (/api/appointments/admin,
/// confirm/cancel/assign-technician). Customer/service/technician names are resolved from bulk lookups
/// (GET /api/users, IServiceCatalogService.GetAllAsync) since AppointmentResponse only carries raw ids.</summary>
public interface IAppointmentDispatchService
{
    Task<List<AppointmentListItemViewModel>> GetAllAsync(string? status, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> ConfirmAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> CancelAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AssignTechnicianViewModel> GetAssignTechnicianFormAsync(Guid appointmentId, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> AssignTechnicianAsync(AssignTechnicianViewModel model, CancellationToken cancellationToken = default);
    Task<RecordWorkOrderViewModel?> GetRecordWorkOrderFormAsync(Guid appointmentId, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> RecordWorkOrderAsync(RecordWorkOrderViewModel model, CancellationToken cancellationToken = default);
    Task<WorkOrderDetailViewModel?> GetWorkOrderDetailAsync(Guid appointmentId, CancellationToken cancellationToken = default);
}
