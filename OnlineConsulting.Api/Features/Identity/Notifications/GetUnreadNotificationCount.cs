using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Notifications.GetUnreadNotificationCount;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Identity.Notifications;

public class GetUnreadNotificationCount : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/notifications/unread-count", Handle)
            .WithTags("Identity/Notifications")
            .RequireAuthorization()
            .WithName("GetUnreadNotificationCount")
            .WithDescription("Returns how many of the current user's notifications are unread (the bell badge).");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext)
        => (await sender.Send(new GetCurrentUserQuery())
                .BindAsync(user => sender.Send(new GetUnreadNotificationCountQuery(user.Id))))
            .ToEnvelopedResult(httpContext);
}
