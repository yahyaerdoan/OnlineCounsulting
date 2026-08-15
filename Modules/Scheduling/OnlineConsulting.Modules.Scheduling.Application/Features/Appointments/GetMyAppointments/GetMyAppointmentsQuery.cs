using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.Scheduling.Application.Contracts;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.GetMyAppointments;

public record GetMyAppointmentsQuery(Guid UserId, PageRequest PageRequest) : IRequest<OperationDataResult<Paginate<AppointmentResponse>>>;

public class GetMyAppointmentsHandler(IAppointmentRepository repository)
    : IRequestHandler<GetMyAppointmentsQuery, OperationDataResult<Paginate<AppointmentResponse>>>
{
    public async Task<OperationDataResult<Paginate<AppointmentResponse>>> Handle(GetMyAppointmentsQuery request, CancellationToken cancellationToken)
    {
        var appointments = await repository.GetListAsync(a => a.UserId == request.UserId, orderBy: q => q.OrderByDescending(a => a.ScheduledStart), index: request.PageRequest.PageIndex, size: request.PageRequest.PageSize, cancellationToken: cancellationToken);

        var response = new Paginate<AppointmentResponse>
        {
            Items = [.. appointments.Items.Select(AppointmentResponse.FromDomain)],
            Index = appointments.Index,
            Size = appointments.Size,
            Count = appointments.Count,
            Pages = appointments.Pages,
        };

        return Result.Success(response, "Appointments retrieved successfully.");
    }
}
