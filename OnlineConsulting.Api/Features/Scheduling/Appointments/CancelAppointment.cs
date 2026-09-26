using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.CancelAppointment;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Scheduling.Appointments;

public class CancelAppointment : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/appointments/{id:guid}/cancel", Handle)
            .WithTags("Scheduling/Appointments")
            .RequireAuthorization()
            .WithName("CancelAppointment")
            .WithDescription("Cancels the current user's own pending or confirmed appointment.");
    }

    private static async Task<IResult> Handle(Guid id, ISender sender, HttpContext httpContext)
        => (await sender.Send(new GetCurrentUserQuery())
                .BindAsync(user => sender.Send(new CancelAppointmentCommand(id, user.Id))))
            .ToEnvelopedResult(httpContext);
}
