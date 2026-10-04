using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Scheduling.Application.Features.AppointmentMediaItems.AddAppointmentMediaItem;
using OnlineConsulting.SharedKernel.CurrentUser;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Scheduling.AppointmentMediaItems;

public class AddAppointmentMediaItem : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/appointments/media-items", Handle)
            .WithTags("Scheduling/Appointments")
            .RequireAuthorization()
            .WithName("AddAppointmentMediaItem")
            .WithDescription("Attaches an already-uploaded photo/video of the issue to one of the current user's own appointments, for the technician to review before the visit.");
    }

    private static async Task<IResult> Handle(ICurrentUserAccessor currentUser, [FromBody] AddAppointmentMediaItemRequest request, ISender sender, HttpContext httpContext)
        => (await sender.Send(request.ToCommand(currentUser.RequiredId())))
            .ToEnvelopedResult(httpContext);
}

public record AddAppointmentMediaItemRequest(Guid AppointmentId, Guid MediaAssetId, int DisplayOrder = 0)
{
    public AddAppointmentMediaItemCommand ToCommand(Guid userId) => new(userId, AppointmentId, MediaAssetId, DisplayOrder);
}
