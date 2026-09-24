using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Scheduling.Application.Features.AppointmentMediaItems.Abstractions;
using OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.Abstractions;
using OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.Rules;
using OnlineConsulting.Modules.Scheduling.Domain;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using ResultHandler.Functional;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Scheduling.Application.Features.AppointmentMediaItems.AddAppointmentMediaItem;

/// <summary>UserId comes from the authenticated caller, never the client; filtering by it makes a stranger's appointment id look like a nonexistent one.</summary>
public record AddAppointmentMediaItemCommand(Guid UserId, Guid AppointmentId, Guid MediaAssetId, int DisplayOrder = 0) : IRequest<OperationDataResult<Guid>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [];
}

public class AddAppointmentMediaItemHandler(IAppointmentMediaItemRepository mediaItemRepository, IAppointmentRepository appointmentRepository) : IRequestHandler<AddAppointmentMediaItemCommand, OperationDataResult<Guid>>
{
    public async Task<OperationDataResult<Guid>> Handle(AddAppointmentMediaItemCommand request, CancellationToken cancellationToken)
    {
        var appointmentExists = await appointmentRepository.AnyAsync(a => a.Id == request.AppointmentId && a.UserId == request.UserId, cancellationToken: cancellationToken);

        if (!appointmentExists)
        {
            return AppointmentBusinessRules.AppointmentNotFound(request.AppointmentId).ToErrorDataResult<Guid>();
        }

        var entity = new AppointmentMediaItem
        {
            AppointmentId = request.AppointmentId,
            MediaAssetId = request.MediaAssetId,
            DisplayOrder = request.DisplayOrder,
        };

        _ = await mediaItemRepository.AddAsync(entity);

        return Result.Created(entity.Id, "Appointment media item added successfully.");
    }
}
