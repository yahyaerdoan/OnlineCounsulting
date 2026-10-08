using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Notifications.MarkNotificationRead;
using OnlineConsulting.SharedKernel.CurrentUser;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Identity.Notifications;

public class MarkNotificationRead : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/notifications/{id:guid}/read", Handle)
            .WithTags("Identity/Notifications")
            .RequireAuthorization()
            .WithName("MarkNotificationRead")
            .WithDescription("Marks one of the current user's notifications as read.")
            .ProducesEnveloped();
    }

    private static async Task<IResult> Handle(ICurrentUserAccessor currentUser, Guid id, ISender sender, HttpContext httpContext)
        => (await sender.Send(new MarkNotificationReadCommand(currentUser.RequiredId(), id)))
            .ToEnvelopedResult(httpContext);
}
