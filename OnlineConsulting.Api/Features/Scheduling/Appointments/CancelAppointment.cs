using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.CancelAppointment;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Scheduling.Appointments;

public class CancelAppointment : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/appointments/{id:guid}/cancel", Handle)
            .WithTags("Scheduling/Appointments")
            .RequireAuthorization()
            .WithName("CancelAppointment")
            .WithDescription("Cancels the current user's own pending or confirmed appointment.");
    }

    private static async Task<IResult> Handle(Guid id, ISender sender, HttpContext httpContext)
    {
        var currentUser = await sender.Send(new GetCurrentUserQuery());
        if (!currentUser.IsSuccessful || currentUser.Data is null)
            return currentUser.ToEnvelopedResult(httpContext);

        var result = await sender.Send(new CancelAppointmentCommand(id, currentUser.Data.Id));
        return result.ToEnvelopedResult(httpContext);
    }
}
