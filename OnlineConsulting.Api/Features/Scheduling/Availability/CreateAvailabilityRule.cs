using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Scheduling.Application.Features.Availability.CreateAvailabilityRule;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Scheduling.Availability;

public class CreateAvailabilityRule : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/scheduling/availability-rules", Handle)
            .WithTags("Scheduling/Availability")
            .RequireAuthorization()
            .WithName("CreateAvailabilityRule")
            .WithDescription("Tenant/admin: adds a recurring weekly working-hours window that appointments can be booked into.");
    }

    private static async Task<IResult> Handle([FromBody] CreateAvailabilityRuleCommand command, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(command);
        return result.ToEnvelopedResult(httpContext);
    }
}
