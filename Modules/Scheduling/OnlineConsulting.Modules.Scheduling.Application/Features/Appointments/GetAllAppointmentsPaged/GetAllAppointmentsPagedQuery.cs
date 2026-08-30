using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Dynamics.Dynamic;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.Scheduling.Application.Common;
using OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.Abstractions;
using OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.Contracts;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.GetAllAppointmentsPaged;

/// <summary>ServerDataTable-friendly sibling of GetAllAppointmentsQuery - that one is a GET-bound flat/filtered list, this one is a POST /query DynamicQuery endpoint the dispatch board's table can bind to directly.</summary>
public record GetAllAppointmentsPagedQuery(PageRequest PageRequest, DynamicQuery? DynamicQuery = null)
    : IRequest<OperationDataResult<Paginate<AppointmentResponse>>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [SchedulingOperationClaims.Admin, SchedulingOperationClaims.Write, SchedulingOperationClaims.Read];
}

public class GetAllAppointmentsPagedHandler(IAppointmentRepository repository)
    : IRequestHandler<GetAllAppointmentsPagedQuery, OperationDataResult<Paginate<AppointmentResponse>>>
{
    public async Task<OperationDataResult<Paginate<AppointmentResponse>>> Handle(GetAllAppointmentsPagedQuery request, CancellationToken cancellationToken)
    {
        var paged = await repository.Query().ToDynamicPaginateAsync(request.PageRequest, request.DynamicQuery, defaultOrderBy: a => a.ScheduledStart, tieBreaker: a => a.Id, cancellationToken);

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
