using Hateoas.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.CreateAppointment;
using OnlineConsulting.Modules.Scheduling.Domain;
using OnlineConsulting.SharedKernel.CurrentUser;
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
            .WithDescription("Books a service (pass serviceId) or requests a generic meeting with the tenant (omit serviceId).")
            .ProducesEnveloped<Guid>(StatusCodes.Status201Created);
    }

    private static async Task<IResult> Handle(ICurrentUserAccessor currentUser, [FromBody] CreateAppointmentRequest request, ISender sender, HttpContext httpContext)
        => (await sender.Send(request.ToCommand(currentUser.RequiredId())))
            .ToEnvelopedResult(httpContext);
}

public record CreateAppointmentRequest(Guid? ServiceId, DateTimeOffset ScheduledStart, DateTimeOffset ScheduledEnd, string? CustomerNote, string? ServiceAddress = null,
    string MeetingType = AppointmentMeetingTypes.InPerson, string? Topic = null)
{
    public CreateAppointmentCommand ToCommand(Guid userId) => new(userId, ServiceId, ScheduledStart, ScheduledEnd, CustomerNote, ServiceAddress, MeetingType, Topic);
}
