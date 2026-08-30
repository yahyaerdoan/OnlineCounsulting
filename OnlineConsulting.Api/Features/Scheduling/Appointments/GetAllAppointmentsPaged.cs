using Core.PersistenceLayer.Dynamics.Dynamic;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.GetAllAppointmentsPaged;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Scheduling.Appointments;

public class GetAllAppointmentsPaged : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/appointments/admin/query", Handle)
            .WithTags("Scheduling/Appointments")
            .RequireAuthorization()
            .WithName("GetAllAppointmentsPaged")
            .WithDescription("Admin/dispatch listing of every appointment for the tenant, paginated (?index=&size=), optionally filtered/sorted via a DynamicQuery body.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext, [AsParameters] ListQueryParameters query, [FromBody] DynamicQuery? dynamicQuery)
    {
        var result = await sender.Send(new GetAllAppointmentsPagedQuery(query.ToPageRequest(), dynamicQuery));
        return result.ToEnvelopedResult(httpContext);
    }
}
