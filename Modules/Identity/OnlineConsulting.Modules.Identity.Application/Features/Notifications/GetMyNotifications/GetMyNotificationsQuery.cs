using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.Identity.Application.Features.Notifications.Abstractions;
using OnlineConsulting.Modules.Identity.Application.Features.Notifications.Contracts;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Identity.Application.Features.Notifications.GetMyNotifications;

/// <summary>The caller's inbox, newest first. UserId is resolved server-side from the authenticated caller.</summary>
public record GetMyNotificationsQuery(Guid UserId, PageRequest PageRequest) : IRequest<OperationDataResult<Paginate<UserNotificationResponse>>>;

public class GetMyNotificationsHandler(IUserNotificationRepository repository)
    : IRequestHandler<GetMyNotificationsQuery, OperationDataResult<Paginate<UserNotificationResponse>>>
{
    public async Task<OperationDataResult<Paginate<UserNotificationResponse>>> Handle(GetMyNotificationsQuery request, CancellationToken cancellationToken)
    {
        var size = request.PageRequest.PageSize;
        var (items, count) = await repository.GetPageAsync(request.UserId, request.PageRequest.PageIndex, size, cancellationToken);

        var response = new Paginate<UserNotificationResponse>
        {
            Items = [.. items.Select(UserNotificationResponse.FromDomain)],
            Index = request.PageRequest.PageIndex,
            Size = size,
            Count = count,
            Pages = size > 0 ? (int)Math.Ceiling(count / (double)size) : 0,
        };

        return Result.Success(response, "Notifications retrieved successfully.");
    }
}
