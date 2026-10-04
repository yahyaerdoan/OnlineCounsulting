using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Scheduling.Application.Common;
using OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.Abstractions;
using OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.Rules;
using OnlineConsulting.Modules.Scheduling.Application.Features.WorkOrders.Abstractions;
using OnlineConsulting.Modules.Scheduling.Application.Features.WorkOrders.Rules;
using OnlineConsulting.Modules.Scheduling.Domain;
using OnlineConsulting.SharedKernel.Billing;
using OnlineConsulting.SharedKernel.Catalog;
using OnlineConsulting.SharedKernel.Transactions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using ResultHandler.Functional;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Scheduling.Application.Features.WorkOrders.CreateWorkOrder;

/// <summary>Recording a WorkOrder is what completes the Appointment - no separate CompleteAppointment command, so the two stay in sync; hence ISchedulingTransactionRequest, ICommerceTransactionRequest.
/// Charges become the customer's invoice; none means the visit isn't billed (e.g. warranty work).</summary>
public record CreateWorkOrderCommand(Guid AppointmentId,
                                     Guid TechnicianUserId,
                                     string? PartsUsed,
                                     string? TechnicianNotes,
                                     DateTimeOffset? CompletedAt,
                                     Guid? EquipmentId = null,
                                     IReadOnlyList<WorkOrderChargeInput>? Charges = null)
    : IRequest<OperationDataResult<Guid>>, ISecureAddRequest, ISchedulingTransactionRequest
{
    [JsonIgnore]
    public string[] Roles => [SchedulingOperationClaims.Admin, SchedulingOperationClaims.Write, SchedulingOperationClaims.Add];
}

public class CreateWorkOrderHandler(IWorkOrderRepository workOrderRepository,
                                    IAppointmentRepository appointmentRepository,
                                    IAppointmentNotifier notifier,
                                    IServiceInvoiceIssuer invoiceIssuer,
                                    IServiceCatalogReader catalogReader)
    : IRequestHandler<CreateWorkOrderCommand, OperationDataResult<Guid>>
{
    public async Task<OperationDataResult<Guid>> Handle(CreateWorkOrderCommand request, CancellationToken cancellationToken)
    {
        var appointment = await appointmentRepository.GetAsync(a => a.Id == request.AppointmentId, cancellationToken: cancellationToken);

        if (appointment is null)
        {
            return AppointmentBusinessRules.AppointmentNotFound(request.AppointmentId).ToErrorDataResult<Guid>();
        }

        if (appointment.Status == AppointmentStatuses.Cancelled)
        {
            return Result.Conflict<Guid>(SchedulingMessages.CannotRecordWorkOrderForCancelledAppointment);
        }

        var alreadyExists = await workOrderRepository.AnyAsync(w => w.AppointmentId == request.AppointmentId, cancellationToken: cancellationToken);

        if (alreadyExists)
        {
            return WorkOrderBusinessRules.WorkOrderAlreadyExistsForAppointment().ToErrorDataResult<Guid>();
        }

        if (appointment.IsClosed)
        {
            return Result.Conflict<Guid>(SchedulingMessages.AppointmentAlreadyClosed);
        }

        var workOrder = new WorkOrder
        {
            AppointmentId = request.AppointmentId,
            TechnicianUserId = request.TechnicianUserId,
            PartsUsed = request.PartsUsed,
            TechnicianNotes = request.TechnicianNotes,
            CompletedAt = request.CompletedAt ?? DateTimeOffset.UtcNow,
            EquipmentId = request.EquipmentId,
        };

        _ = await workOrderRepository.AddAsync(workOrder, cancellationToken: cancellationToken);

        appointment.Complete();

        _ = await appointmentRepository.UpdateAsync(appointment, cancellationToken: cancellationToken);

        await notifier.CompletedAsync(appointment, cancellationToken);

        if (request.Charges is { Count: > 0 } charges)
        {
            var serviceTitle = appointment.ServiceId is { } serviceId && await catalogReader.GetAsync(serviceId, cancellationToken) is { } service
                ? service.Title
                : "Consultation";

            _ = await invoiceIssuer.IssueForCompletedVisitAsync(new ServiceInvoiceRequest(appointment.Id, appointment.UserId, serviceTitle, appointment.ServiceAddress,
                [.. charges.Select(c => new InvoiceLineInput(c.Description, c.Quantity, c.UnitPrice, c.TaxRate))]), cancellationToken);
        }

        return Result.Created(workOrder.Id, "Work order recorded successfully.");
    }
}
