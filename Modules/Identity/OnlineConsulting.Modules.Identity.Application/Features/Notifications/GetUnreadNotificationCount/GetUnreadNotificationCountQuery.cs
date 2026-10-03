using MediatR;
using OnlineConsulting.Modules.Identity.Application.Features.Notifications.Abstractions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Identity.Application.Features.Notifications.GetUnreadNotificationCount;

/// <summary>Badge count for the bell icon.</summary>
public record GetUnreadNotificationCountQuery(Guid UserId) : IRequest<OperationDataResult<int>>;

public class GetUnreadNotificationCountHandler(IUserNotificationRepository repository) : IRequestHandler<GetUnreadNotificationCountQuery, OperationDataResult<int>>
{
    public async Task<OperationDataResult<int>> Handle(GetUnreadNotificationCountQuery request, CancellationToken cancellationToken) =>
        Result.Success(await repository.CountUnreadAsync(request.UserId, cancellationToken), "Unread notification count retrieved successfully.");
}
