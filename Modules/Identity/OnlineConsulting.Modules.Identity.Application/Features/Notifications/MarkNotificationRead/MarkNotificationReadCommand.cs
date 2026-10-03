using MediatR;
using OnlineConsulting.Modules.Identity.Application.Features.Notifications.Abstractions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Identity.Application.Features.Notifications.MarkNotificationRead;

/// <summary>Marks one of the caller's own notifications read; someone else's id is reported as not found.</summary>
public record MarkNotificationReadCommand(Guid UserId, Guid NotificationId) : IRequest<OperationResult>;

public class MarkNotificationReadHandler(IUserNotificationRepository repository) : IRequestHandler<MarkNotificationReadCommand, OperationResult>
{
    public async Task<OperationResult> Handle(MarkNotificationReadCommand request, CancellationToken cancellationToken) =>
        await repository.MarkReadAsync(request.UserId, request.NotificationId, cancellationToken)
            ? Result.Success("Notification marked as read.")
            : Result.NotFound($"Notification {request.NotificationId} was not found.");
}
