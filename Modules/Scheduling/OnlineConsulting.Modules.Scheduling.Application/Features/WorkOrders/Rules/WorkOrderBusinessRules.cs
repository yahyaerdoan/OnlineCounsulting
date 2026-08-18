using OnlineConsulting.Modules.Scheduling.Application.Common;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Scheduling.Application.Features.WorkOrders.Rules;

public static class WorkOrderBusinessRules
{
    public static OperationResult WorkOrderNotFoundForAppointment(Guid appointmentId) =>
        Result.NotFound(string.Format(SchedulingMessages.WorkOrderNotFoundForAppointmentFormat, appointmentId));

    public static OperationResult WorkOrderNotFound(Guid workOrderId) =>
        Result.NotFound(string.Format(SchedulingMessages.WorkOrderNotFoundFormat, workOrderId));

    public static OperationResult WorkOrderAlreadyExistsForAppointment() =>
        Result.BadRequest(SchedulingMessages.WorkOrderAlreadyExistsForAppointment);
}
