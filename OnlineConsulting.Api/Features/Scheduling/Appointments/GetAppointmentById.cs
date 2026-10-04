using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.GetAppointmentById;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Scheduling.Appointments;

public class GetAppointmentById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/appointments/{id:guid}", Handle)
            .WithTags("Scheduling/Appointments")
            .RequireAuthorization()
            .WithName("GetAppointmentById")
            .WithDescription("Returns an appointment the current user owns as customer or is assigned to as technician, including its pre-diagnosis media gallery.");
    }

    private static async Task<IResult> Handle(Guid id, ISender sender, HttpContext httpContext)
        => (await sender.Send(new GetCurrentUserQuery())
                .BindAsync(user => sender.Send(new GetAppointmentByIdQuery(id, user.Id))))
            .ToEnvelopedResult(httpContext);
}
