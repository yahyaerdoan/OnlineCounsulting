using MediatR;
using OnlineConsulting.Modules.Scheduling.Application.Features.AppointmentMediaItems.Abstractions;
using OnlineConsulting.Modules.Scheduling.Application.Features.AppointmentMediaItems.Contracts;
using OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.Abstractions;
using OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.Contracts;
using OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.Rules;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using ResultHandler.Functional;

namespace OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.GetAppointmentById;

/// <summary>Scoped to the caller's own appointment as customer or as the assigned technician - same authorization shape as TechnicianTrackingHub.JoinAppointmentTracking.</summary>
public record GetAppointmentByIdQuery(Guid Id, Guid UserId) : IRequest<OperationDataResult<AppointmentResponse>>;

public class GetAppointmentByIdHandler(IAppointmentRepository appointmentRepository, IAppointmentMediaItemRepository mediaItemRepository)
    : IRequestHandler<GetAppointmentByIdQuery, OperationDataResult<AppointmentResponse>>
{
    public async Task<OperationDataResult<AppointmentResponse>> Handle(GetAppointmentByIdQuery request, CancellationToken cancellationToken)
    {
        var appointment = await appointmentRepository.GetAsync(
            a => a.Id == request.Id && (a.UserId == request.UserId || a.AssignedTechnicianUserId == request.UserId),
            cancellationToken: cancellationToken);
        if (appointment is null)
        {
            return AppointmentBusinessRules.AppointmentNotFound(request.Id).ToErrorDataResult<AppointmentResponse>();
        }

        var mediaItems = await mediaItemRepository.GetListAsync(m => m.AppointmentId == appointment.Id, orderBy: q => q.OrderBy(m => m.DisplayOrder), size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);

        return Result.Success(AppointmentResponse.FromDomain(appointment, [.. mediaItems.Items.Select(AppointmentMediaItemResponse.FromDomain)]), "Appointment retrieved successfully.");
    }
}
