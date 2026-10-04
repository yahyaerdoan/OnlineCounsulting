using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Notifications.GetMyNotifications;
using OnlineConsulting.SharedKernel.CurrentUser;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Identity.Notifications;

public class GetMyNotifications : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/notifications", Handle)
            .WithTags("Identity/Notifications")
            .RequireAuthorization()
            .WithName("GetMyNotifications")
            .WithDescription("Returns the current user's in-app notifications, newest first, paginated.");
    }

    private static async Task<IResult> Handle(ICurrentUserAccessor currentUser, ISender sender, HttpContext httpContext, int? index = null, int? size = null)
        => (await sender.Send(new GetMyNotificationsQuery(currentUser.RequiredId(), PageRequestFactory.Create(index, size))))
            .ToEnvelopedResult(httpContext);
}
