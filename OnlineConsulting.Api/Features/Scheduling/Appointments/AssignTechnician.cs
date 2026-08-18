using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.AssignTechnician;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Scheduling.Appointments;

public class AssignTechnician : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/appointments/{id:guid}/assign-technician", Handle)
            .WithTags("Scheduling/Appointments")
            .RequireAuthorization()
            .WithName("AssignTechnician")
            .WithDescription("Dispatches a technician to an appointment (admin) - authorizes that technician to push live location updates for it.");
    }

    private static async Task<IResult> Handle(Guid id, [FromBody] AssignTechnicianCommand command, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(command with { Id = id });
        return result.ToEnvelopedResult(httpContext);
    }
}
