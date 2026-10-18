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
using OnlineConsulting.SharedKernel.Catalog;
using OnlineConsulting.SharedKernel.Identity;
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

public class ListAppointmentsHandler(IAppointmentRepository repository, IUserContactReader contactReader, IServiceCatalogReader catalogReader)
    : IRequestHandler<ListAppointmentsQuery, OperationDataResult<Paginate<AppointmentResponse>>>
{
    public async Task<OperationDataResult<Paginate<AppointmentResponse>>> Handle(ListAppointmentsQuery request, CancellationToken cancellationToken)
    {
        var paged = await repository.Query().ToDynamicPaginateAsync(request, defaultOrderBy: a => a.CreatedDate, tieBreaker: a => a.Id, defaultDescending: true, cancellationToken: cancellationToken);

        var customers = (await contactReader.GetContactsAsync([.. paged.Items.Select(a => a.UserId).Distinct()], cancellationToken)).ToDictionary(c => c.Id);
        var services = await catalogReader.GetManyAsync(paged.Items.Where(a => a.ServiceId is not null).Select(a => a.ServiceId.GetValueOrDefault()).Distinct(), cancellationToken);

        var response = new Paginate<AppointmentResponse>
        {
            Items = [.. paged.Items.Select(a => AppointmentResponse.FromDomain(a, customer: customers.GetValueOrDefault(a.UserId), serviceTitle: ServiceTitle(a, services)))],
            Index = paged.Index,
            Size = paged.Size,
            Count = paged.Count,
            Pages = paged.Pages,
        };

        return Result.Success(response, "Appointments retrieved successfully.");
    }

    /// <summary>Null service means a general consultation.</summary>
    private static string? ServiceTitle(Appointment appointment, IReadOnlyDictionary<Guid, ServiceCatalogEntry> services) =>
        appointment.ServiceId is { } serviceId ? services.GetValueOrDefault(serviceId)?.Title : "Consultation";
}
