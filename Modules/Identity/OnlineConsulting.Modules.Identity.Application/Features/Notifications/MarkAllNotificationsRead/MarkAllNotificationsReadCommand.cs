using MediatR;
using OnlineConsulting.Modules.Identity.Application.Features.Notifications.Abstractions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Identity.Application.Features.Notifications.MarkAllNotificationsRead;

public record MarkAllNotificationsReadCommand(Guid UserId) : IRequest<OperationResult>;

public class MarkAllNotificationsReadHandler(IUserNotificationRepository repository) : IRequestHandler<MarkAllNotificationsReadCommand, OperationResult>
{
    public async Task<OperationResult> Handle(MarkAllNotificationsReadCommand request, CancellationToken cancellationToken)
    {
        await repository.MarkAllReadAsync(request.UserId, cancellationToken);
        return Result.Success("All notifications marked as read.");
    }
}
