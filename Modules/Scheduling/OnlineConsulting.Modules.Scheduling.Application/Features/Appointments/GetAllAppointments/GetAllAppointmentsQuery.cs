using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.Scheduling.Application.Common;
using OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.Abstractions;
using OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.Contracts;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.GetAllAppointments;

/// <summary>Admin/dispatch-side listing of every appointment for the tenant, optionally filtered by status - the
/// counterpart to GetMyAppointmentsQuery's owner-scoped list, needed so an admin can actually find the
/// Pending appointments ConfirmAppointment/AssignTechnician operate on.</summary>
public record GetAllAppointmentsQuery(string? Status, PageRequest PageRequest) : IRequest<OperationDataResult<Paginate<AppointmentResponse>>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [SchedulingOperationClaims.Admin, SchedulingOperationClaims.Write];
}

public class GetAllAppointmentsHandler(IAppointmentRepository repository)
    : IRequestHandler<GetAllAppointmentsQuery, OperationDataResult<Paginate<AppointmentResponse>>>
{
    public async Task<OperationDataResult<Paginate<AppointmentResponse>>> Handle(GetAllAppointmentsQuery request, CancellationToken cancellationToken)
    {
        var appointments = string.IsNullOrWhiteSpace(request.Status)
            ? await repository.GetListAsync(
                orderBy: q => q.OrderByDescending(a => a.ScheduledStart),
                index: request.PageRequest.PageIndex, size: request.PageRequest.PageSize, cancellationToken: cancellationToken)
            : await repository.GetListAsync(
                predicate: a => a.Status == request.Status,
                orderBy: q => q.OrderByDescending(a => a.ScheduledStart),
                index: request.PageRequest.PageIndex, size: request.PageRequest.PageSize, cancellationToken: cancellationToken);

        var response = new Paginate<AppointmentResponse>
        {
            Items = [.. appointments.Items.Select(a => AppointmentResponse.FromDomain(a))],
            Index = appointments.Index,
            Size = appointments.Size,
            Count = appointments.Count,
            Pages = appointments.Pages,
        };

        return Result.Success(response, "Appointments retrieved successfully.");
    }
}
