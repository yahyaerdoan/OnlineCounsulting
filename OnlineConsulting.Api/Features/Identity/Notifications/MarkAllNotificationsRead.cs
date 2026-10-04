using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Notifications.MarkAllNotificationsRead;
using OnlineConsulting.SharedKernel.CurrentUser;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Identity.Notifications;

public class MarkAllNotificationsRead : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/notifications/read-all", Handle)
            .WithTags("Identity/Notifications")
            .RequireAuthorization()
            .WithName("MarkAllNotificationsRead")
            .WithDescription("Marks all of the current user's notifications as read.");
    }

    private static async Task<IResult> Handle(ICurrentUserAccessor currentUser, ISender sender, HttpContext httpContext)
        => (await sender.Send(new MarkAllNotificationsReadCommand(currentUser.RequiredId())))
            .ToEnvelopedResult(httpContext);
}
