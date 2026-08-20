using MediatR;
using OnlineConsulting.Modules.Scheduling.Application.Features.WorkOrders.Abstractions;
using OnlineConsulting.Modules.Scheduling.Application.Features.WorkOrders.Contracts;
using OnlineConsulting.Modules.Scheduling.Application.Features.WorkOrders.Rules;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using ResultHandler.Functional;

namespace OnlineConsulting.Modules.Scheduling.Application.Features.WorkOrders.GetWorkOrderByAppointmentId;

public record GetWorkOrderByAppointmentIdQuery(Guid AppointmentId) : IRequest<OperationDataResult<WorkOrderResponse>>;

public class GetWorkOrderByAppointmentIdHandler(IWorkOrderRepository workOrderRepository, IWorkOrderMediaItemRepository mediaItemRepository)
    : IRequestHandler<GetWorkOrderByAppointmentIdQuery, OperationDataResult<WorkOrderResponse>>
{
    public async Task<OperationDataResult<WorkOrderResponse>> Handle(GetWorkOrderByAppointmentIdQuery request, CancellationToken cancellationToken)
    {
        var workOrder = await workOrderRepository.GetAsync(w => w.AppointmentId == request.AppointmentId, cancellationToken: cancellationToken);
        if (workOrder is null)
        {
            return WorkOrderBusinessRules.WorkOrderNotFoundForAppointment(request.AppointmentId).ToErrorDataResult<WorkOrderResponse>();
        }

        var mediaItems = await mediaItemRepository.GetListAsync(m => m.WorkOrderId == workOrder.Id, size: 100, cancellationToken: cancellationToken);

        return Result.Success(WorkOrderResponse.FromDomain(workOrder, mediaItems.Items), "Work order retrieved successfully.");
    }
}
