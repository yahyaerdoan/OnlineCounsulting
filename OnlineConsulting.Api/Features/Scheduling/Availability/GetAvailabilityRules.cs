using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Scheduling.Application.Features.Availability.GetAvailabilityRules;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Scheduling.Availability;

public class GetAvailabilityRules : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/scheduling/availability-rules", Handle)
            .WithTags("Scheduling/Availability")
            .RequireAuthorization()
            .WithName("GetAvailabilityRules")
            .WithDescription("Tenant/admin: lists the tenant's own recurring working-hours windows.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new GetAvailabilityRulesQuery());
        return result.ToEnvelopedResult(httpContext);
    }
}
