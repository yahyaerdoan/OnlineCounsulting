using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.ConfirmAppointment;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Scheduling.Appointments;

public class ConfirmAppointment : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/appointments/{id:guid}/confirm", Handle)
            .WithTags("Scheduling/Appointments")
            .RequireAuthorization()
            .WithName("ConfirmAppointment")
            .WithDescription("Tenant/admin approval of a pending appointment.");
    }

    private static async Task<IResult> Handle(Guid id, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new ConfirmAppointmentCommand(id));
        return result.ToEnvelopedResult(httpContext);
    }
}
