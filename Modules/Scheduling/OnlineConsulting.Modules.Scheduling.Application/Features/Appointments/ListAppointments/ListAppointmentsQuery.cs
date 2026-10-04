using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Requests.Lists;
using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Dynamics.Dynamic;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.Scheduling.Application.Common;
using OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.Abstractions;
using OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.Contracts;
using OnlineConsulting.Modules.Scheduling.Domain;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.ListAppointments;

/// <summary>ServerDataTable-friendly sibling of GetAllAppointmentsQuery - that one is a GET-bound flat/filtered list, this one is a POST /query DynamicQuery endpoint the dispatch board's table can bind to directly.</summary>
public record ListAppointmentsQuery(PageRequest PageRequest, DynamicQuery? DynamicQuery = null)
    : IRequest<OperationDataResult<Paginate<AppointmentResponse>>>, ISecureAddRequest, IDynamicListRequest
{
    public static IReadOnlySet<string> QueryableFields { get; } = new HashSet<string>([nameof(Appointment.Status), nameof(Appointment.ScheduledStart)]);

    public string[] Roles => [SchedulingOperationClaims.Admin, SchedulingOperationClaims.Write, SchedulingOperationClaims.Read];
}

public class ListAppointmentsHandler(IAppointmentRepository repository)
    : IRequestHandler<ListAppointmentsQuery, OperationDataResult<Paginate<AppointmentResponse>>>
{
    public async Task<OperationDataResult<Paginate<AppointmentResponse>>> Handle(ListAppointmentsQuery request, CancellationToken cancellationToken)
    {
        var paged = await repository.Query().ToDynamicPaginateAsync(request, defaultOrderBy: a => a.CreatedDate, tieBreaker: a => a.Id, defaultDescending: true, cancellationToken: cancellationToken);

        var response = new Paginate<AppointmentResponse>
        {
            Items = [.. paged.Items.Select(a => AppointmentResponse.FromDomain(a))],
            Index = paged.Index,
            Size = paged.Size,
            Count = paged.Count,
            Pages = paged.Pages,
        };

        return Result.Success(response, "Appointments retrieved successfully.");
    }
}
