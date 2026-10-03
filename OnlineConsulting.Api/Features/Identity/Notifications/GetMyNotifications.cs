using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Notifications.GetMyNotifications;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Identity.Notifications;

public class GetMyNotifications : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/notifications", Handle)
            .WithTags("Identity/Notifications")
            .RequireAuthorization()
            .WithName("GetMyNotifications")
            .WithDescription("Returns the current user's in-app notifications, newest first, paginated.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext, int? index = null, int? size = null)
        => (await sender.Send(new GetCurrentUserQuery())
                .BindAsync(user => sender.Send(new GetMyNotificationsQuery(user.Id, PageRequestFactory.Create(index, size)))))
            .ToEnvelopedResult(httpContext);
}
