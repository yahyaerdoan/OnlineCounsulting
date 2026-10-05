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
        _ = app.MapPost("/scheduling/availability-rules", Handle)
            .WithTags("Scheduling/Availability")
            .RequireAuthorization()
            .WithName("CreateAvailabilityRule")
            .WithDescription("Tenant/admin: adds a recurring weekly working-hours window that appointments can be booked into.")
            .ProducesEnveloped<Guid>(StatusCodes.Status201Created);
    }

    private static async Task<IResult> Handle([FromBody] CreateAvailabilityRuleRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToEnvelopedResult(httpContext);
    }
}

public record CreateAvailabilityRuleRequest(DayOfWeek DayOfWeek, TimeSpan StartTime, TimeSpan EndTime, int SlotDurationMinutes)
{
    public CreateAvailabilityRuleCommand ToCommand() => new(DayOfWeek, StartTime, EndTime, SlotDurationMinutes);
}
