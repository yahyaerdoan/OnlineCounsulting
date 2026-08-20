using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Scheduling.Application.Features.WorkOrders.GetWorkOrderByAppointmentId;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Scheduling.WorkOrders;

public class GetWorkOrderByAppointmentId : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/appointments/{appointmentId:guid}/work-order", Handle)
            .WithTags("Scheduling/WorkOrders")
            .RequireAuthorization()
            .WithName("GetWorkOrderByAppointmentId")
            .WithDescription("Returns the work order (with its before/after media gallery) recorded against an appointment, if one exists.");
    }

    private static async Task<IResult> Handle(Guid appointmentId, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new GetWorkOrderByAppointmentIdQuery(appointmentId));
        return result.ToEnvelopedResult(httpContext);
    }
}
