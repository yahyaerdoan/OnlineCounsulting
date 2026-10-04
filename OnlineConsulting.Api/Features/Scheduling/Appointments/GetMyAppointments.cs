using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.GetMyAppointments;
using OnlineConsulting.SharedKernel.CurrentUser;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Scheduling.Appointments;

public class GetMyAppointments : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/appointments/mine", Handle)
            .WithTags("Scheduling/Appointments")
            .RequireAuthorization()
            .WithName("GetMyAppointments")
            .WithDescription("Returns the current user's own appointments, paginated.");
    }

    private static async Task<IResult> Handle(ICurrentUserAccessor currentUser, ISender sender, HttpContext httpContext, int? index = null, int? size = null)
        => (await sender.Send(new GetMyAppointmentsQuery(currentUser.RequiredId(), PageRequestFactory.Create(index, size))))
            .ToEnvelopedResult(httpContext);
}
