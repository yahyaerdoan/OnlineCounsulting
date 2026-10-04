using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Notifications.GetUnreadNotificationCount;
using OnlineConsulting.SharedKernel.CurrentUser;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Identity.Notifications;

public class GetUnreadNotificationCount : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/notifications/unread-count", Handle)
            .WithTags("Identity/Notifications")
            .RequireAuthorization()
            .WithName("GetUnreadNotificationCount")
            .WithDescription("Returns how many of the current user's notifications are unread (the bell badge).");
    }

    private static async Task<IResult> Handle(ICurrentUserAccessor currentUser, ISender sender, HttpContext httpContext)
        => (await sender.Send(new GetUnreadNotificationCountQuery(currentUser.RequiredId())))
            .ToEnvelopedResult(httpContext);
}
