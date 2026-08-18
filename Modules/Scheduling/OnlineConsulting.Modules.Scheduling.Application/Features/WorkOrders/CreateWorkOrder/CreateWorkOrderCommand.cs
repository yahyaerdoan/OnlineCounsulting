using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Scheduling.Application.Common;
using OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.Abstractions;
using OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.Constants;
using OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.Rules;
using OnlineConsulting.Modules.Scheduling.Application.Features.WorkOrders.Abstractions;
using OnlineConsulting.Modules.Scheduling.Application.Features.WorkOrders.Rules;
using OnlineConsulting.Modules.Scheduling.Domain;
using OnlineConsulting.SharedKernel.Notifications;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using ResultHandler.Functional;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Scheduling.Application.Features.WorkOrders.CreateWorkOrder;

/// <summary>Recording a WorkOrder is what completes the Appointment - no separate "CompleteAppointment" command exists, since letting the two be updated independently risks a WorkOrder existing against an Appointment that's still Pending/Confirmed. Two SaveChanges (WorkOrder add + Appointment update) in one handler, hence ITransactionAddRequest.</summary>
public record CreateWorkOrderCommand(Guid AppointmentId, Guid TechnicianUserId, string? PartsUsed, string? TechnicianNotes, DateTimeOffset? CompletedAt, Guid? EquipmentId = null)
    : IRequest<OperationDataResult<Guid>>, ISecureAddRequest, ITransactionAddRequest
{
    [JsonIgnore]
    public string[] Roles => [SchedulingOperationClaims.Admin, SchedulingOperationClaims.Write];
}

public class CreateWorkOrderHandler(IWorkOrderRepository workOrderRepository, IAppointmentRepository appointmentRepository, IPushNotificationSender pushNotificationSender)
    : IRequestHandler<CreateWorkOrderCommand, OperationDataResult<Guid>>
{
    public async Task<OperationDataResult<Guid>> Handle(CreateWorkOrderCommand request, CancellationToken cancellationToken)
    {
        var appointment = await appointmentRepository.GetAsync(a => a.Id == request.AppointmentId, cancellationToken: cancellationToken);
        if (appointment is null)
            return AppointmentBusinessRules.AppointmentNotFound(request.AppointmentId).ToErrorDataResult<Guid>();

        var alreadyExists = await workOrderRepository.AnyAsync(w => w.AppointmentId == request.AppointmentId, cancellationToken: cancellationToken);
        if (alreadyExists)
            return WorkOrderBusinessRules.WorkOrderAlreadyExistsForAppointment().ToErrorDataResult<Guid>();

        var workOrder = new WorkOrder
        {
            Id = Guid.NewGuid(),
            AppointmentId = request.AppointmentId,
            TechnicianUserId = request.TechnicianUserId,
            PartsUsed = request.PartsUsed,
            TechnicianNotes = request.TechnicianNotes,
            CompletedAt = request.CompletedAt ?? DateTimeOffset.UtcNow,
            EquipmentId = request.EquipmentId,
        };

        await workOrderRepository.AddAsync(workOrder);

        appointment.Status = AppointmentStatuses.Completed;
        await appointmentRepository.UpdateAsync(appointment);

        await pushNotificationSender.SendToUserAsync(
            appointment.UserId,
            "Service complete",
            "Your appointment has been completed. Thanks for choosing us!",
            new Dictionary<string, string> { ["appointmentId"] = appointment.Id.ToString() },
            cancellationToken);

        return Result.Created(workOrder.Id, "Work order recorded successfully.");
    }
}
