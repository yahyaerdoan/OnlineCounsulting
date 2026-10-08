using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.CancelAppointment;
using OnlineConsulting.SharedKernel.CurrentUser;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Scheduling.Appointments;

public class CancelAppointment : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/appointments/{id:guid}/cancel", Handle)
            .WithTags("Scheduling/Appointments")
            .RequireAuthorization()
            .WithName("CancelAppointment")
            .WithDescription("Cancels the current user's own pending or confirmed appointment.")
            .ProducesEnveloped();
    }

    private static async Task<IResult> Handle(ICurrentUserAccessor currentUser, Guid id, ISender sender, HttpContext httpContext)
        => (await sender.Send(new CancelAppointmentCommand(id, currentUser.RequiredId())))
            .ToEnvelopedResult(httpContext);
}
