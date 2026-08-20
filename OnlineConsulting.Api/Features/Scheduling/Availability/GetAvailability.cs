using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Scheduling.Application.Features.Availability.GetAvailability;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Scheduling.Availability;

public class GetAvailability : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/scheduling/availability", Handle)
            .WithTags("Scheduling/Availability")
            .WithName("GetAvailability")
            .WithDescription("Returns free time slots for the given date, computed from the tenant's availability rules minus existing appointments. Public - no login required to browse open slots.");
    }

    private static async Task<IResult> Handle(DateOnly date, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new GetAvailabilityQuery(date));
        return result.ToEnvelopedResult(httpContext);
    }
}
