using OnlineConsulting.Modules.Scheduling.Application.Features.Constants;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Scheduling.Application.Features.Rules;

public static class AppointmentBusinessRules
{
    public static OperationResult AppointmentNotFound(Guid appointmentId) =>
        Result.NotFound(string.Format(SchedulingMessages.AppointmentNotFoundFormat, appointmentId));

    public static OperationResult SlotNoLongerAvailable() =>
        Result.BadRequest(SchedulingMessages.SlotNoLongerAvailable);
}
