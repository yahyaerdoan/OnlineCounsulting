using Hateoas.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.CreateAppointment;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Scheduling.Appointments;

public class CreateAppointment : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/appointments", Handle)
            .WithTags("Scheduling/Appointments")
            .RequireAuthorization()
            .WithName("CreateAppointment")
            .WithCreatedLocation("GetAppointmentById")
            .WithDescription("Books a service (pass serviceId) or requests a generic meeting with the tenant (omit serviceId).");
    }

    private static async Task<IResult> Handle([FromBody] CreateAppointmentCommand command, ISender sender, HttpContext httpContext)
        => (await sender.Send(new GetCurrentUserQuery())
                .BindAsync(user => sender.Send(command with { UserId = user.Id })))
            .ToEnvelopedResult(httpContext);
}
