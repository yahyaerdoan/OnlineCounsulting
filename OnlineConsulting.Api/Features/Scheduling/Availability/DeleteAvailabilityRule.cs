using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Scheduling.Application.Features.Availability.DeleteAvailabilityRule;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Scheduling.Availability;

public class DeleteAvailabilityRule : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapDelete("/api/scheduling/availability-rules/{id:guid}", Handle)
            .WithTags("Scheduling/Availability")
            .RequireAuthorization()
            .WithName("DeleteAvailabilityRule")
            .WithDescription("Tenant/admin: removes a recurring working-hours window.");
    }

    private static async Task<IResult> Handle(Guid id, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new DeleteAvailabilityRuleCommand(id));
        return result.ToEnvelopedResult(httpContext);
    }
}
