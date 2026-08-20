using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceProcessSteps.CreateServiceProcessStep;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.ServiceProcessSteps;

public class CreateServiceProcessStep : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/site-content/service-process-steps", Handle)
            .WithTags("SiteContent/ServiceProcessSteps")
            .RequireAuthorization()
            .WithName("CreateServiceProcessStep")
            .WithDescription("Creates a step in the \"how you get our service\" homepage section.");
    }

    private static async Task<IResult> Handle([FromBody] CreateServiceProcessStepCommand command, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(command);
        return result.ToEnvelopedResult(httpContext);
    }
}
