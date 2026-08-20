using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.CreateAppointment;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Scheduling.Appointments;

public class CreateAppointment : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/appointments", Handle)
            .WithTags("Scheduling/Appointments")
            .RequireAuthorization()
            .WithName("CreateAppointment")
            .WithDescription("Books a service (pass serviceId) or requests a generic meeting with the tenant (omit serviceId).");
    }

    private static async Task<IResult> Handle([FromBody] CreateAppointmentCommand command, ISender sender, HttpContext httpContext)
    {
        var currentUser = await sender.Send(new GetCurrentUserQuery());
        if (!currentUser.IsSuccessful || currentUser.Data is null)
        {
            return currentUser.ToEnvelopedResult(httpContext);
        }

        var result = await sender.Send(command with { UserId = currentUser.Data.Id, Email = currentUser.Data.Email });
        return result.ToEnvelopedResult(httpContext);
    }
}
