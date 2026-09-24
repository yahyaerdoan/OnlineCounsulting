using MediatR;
using OnlineConsulting.Modules.Scheduling.Application.Features.WorkOrders.Abstractions;
using OnlineConsulting.Modules.Scheduling.Application.Features.WorkOrders.Contracts;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Scheduling.Application.Features.WorkOrders.GetWorkOrdersByEquipmentId;

/// <summary>Service history behind the equipment health panel, most recent first; media items omitted (list view) - fetch via GetWorkOrderByAppointmentId if needed.</summary>
public record GetWorkOrdersByEquipmentIdQuery(Guid EquipmentId) : IRequest<OperationDataResult<List<WorkOrderResponse>>>;

public class GetWorkOrdersByEquipmentIdHandler(IWorkOrderRepository repository) : IRequestHandler<GetWorkOrdersByEquipmentIdQuery, OperationDataResult<List<WorkOrderResponse>>>
{
    public async Task<OperationDataResult<List<WorkOrderResponse>>> Handle(GetWorkOrdersByEquipmentIdQuery request, CancellationToken cancellationToken)
    {
        var workOrders = await repository.GetListAsync(
            w => w.EquipmentId == request.EquipmentId, orderBy: q => q.OrderByDescending(w => w.CompletedAt),
            size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);

        var response = workOrders.Items.Select(w => WorkOrderResponse.FromDomain(w, [])).ToList();

        return Result.Success(response, "Work orders retrieved successfully.");
    }
}
