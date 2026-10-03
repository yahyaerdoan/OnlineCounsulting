using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.CancelAppointmentByStaff;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Scheduling.Appointments;

public class CancelAppointmentByStaff : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/appointments/admin/{id:guid}/cancel", Handle)
            .WithTags("Scheduling/Appointments")
            .RequireAuthorization()
            .WithName("CancelAppointmentByStaff")
            .WithDescription("Cancels any pending or confirmed appointment (admin) and notifies the customer and the assigned technician.");
    }

    private static async Task<IResult> Handle(Guid id, CancelAppointmentByStaffRequest? body, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new CancelAppointmentByStaffCommand(id, body?.Reason));
        return result.ToEnvelopedResult(httpContext);
    }
}

public sealed record CancelAppointmentByStaffRequest(string? Reason);
