using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using OnlineConsulting.Modules.Scheduling.Application.Features.AppointmentMediaItems.AddAppointmentMediaItem;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Scheduling.AppointmentMediaItems;

public class AddAppointmentMediaItem : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/appointments/media-items", Handle)
            .WithTags("Scheduling/Appointments")
            .RequireAuthorization()
            .WithName("AddAppointmentMediaItem")
            .WithDescription("Attaches an already-uploaded photo/video of the issue to one of the current user's own appointments, for the technician to review before the visit.");
    }

    private static async Task<IResult> Handle([FromBody] AddAppointmentMediaItemCommand command, ISender sender, HttpContext httpContext)
    {
        var currentUser = await sender.Send(new GetCurrentUserQuery());
        if (!currentUser.IsSuccessful || currentUser.Data is null)
            return currentUser.ToEnvelopedResult(httpContext);

        var result = await sender.Send(command with { UserId = currentUser.Data.Id });
        return result.ToEnvelopedResult(httpContext);
    }
}
