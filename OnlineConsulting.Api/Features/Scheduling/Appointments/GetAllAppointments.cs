using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.GetAllAppointments;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Scheduling.Appointments;

public class GetAllAppointments : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/appointments/admin", Handle)
            .WithTags("Scheduling/Appointments")
            .RequireAuthorization()
            .WithName("GetAllAppointments")
            .WithDescription("Admin/dispatch listing of every appointment for the tenant, optionally filtered by status.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext, string? status = null, int? index = null, int? size = null)
    {
        var result = await sender.Send(new GetAllAppointmentsQuery(status, PageRequestFactory.Create(index, size)));
        return result.ToEnvelopedResult(httpContext);
    }
}
