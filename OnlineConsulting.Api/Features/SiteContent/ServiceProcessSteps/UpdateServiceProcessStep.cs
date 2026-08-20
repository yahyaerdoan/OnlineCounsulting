using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceProcessSteps.UpdateServiceProcessStep;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.ServiceProcessSteps;

public class UpdateServiceProcessStep : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/api/site-content/service-process-steps/{id:guid}", Handle)
            .WithTags("SiteContent/ServiceProcessSteps")
            .RequireAuthorization()
            .WithName("UpdateServiceProcessStep")
            .WithDescription("Updates a service process step.");
    }

    private static async Task<IResult> Handle(Guid id, [FromBody] UpdateServiceProcessStepCommand command, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(command with { Id = id });
        return result.ToEnvelopedResult(httpContext);
    }
}
