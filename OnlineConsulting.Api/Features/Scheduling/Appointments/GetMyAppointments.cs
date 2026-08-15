using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.GetMyAppointments;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Scheduling.Appointments;

public class GetMyAppointments : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/appointments/mine", Handle)
            .WithTags("Scheduling/Appointments")
            .RequireAuthorization()
            .WithName("GetMyAppointments")
            .WithDescription("Returns the current user's own appointments, paginated.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext, int? index = null, int? size = null)
    {
        var currentUser = await sender.Send(new GetCurrentUserQuery());
        if (!currentUser.IsSuccessful || currentUser.Data is null)
            return currentUser.ToEnvelopedResult(httpContext);

        var result = await sender.Send(new GetMyAppointmentsQuery(currentUser.Data.Id, PageRequestFactory.Create(index, size)));
        return result.ToEnvelopedResult(httpContext);
    }
}
