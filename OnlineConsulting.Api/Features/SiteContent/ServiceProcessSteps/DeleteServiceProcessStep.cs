using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceProcessSteps.DeleteServiceProcessStep;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.ServiceProcessSteps;

public class DeleteServiceProcessStep : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapDelete("/api/site-content/service-process-steps/{id:guid}", Handle)
            .WithTags("SiteContent/ServiceProcessSteps")
            .RequireAuthorization()
            .WithName("DeleteServiceProcessStep")
            .WithDescription("Deletes a service process step.");
    }

    private static async Task<IResult> Handle(Guid id, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new DeleteServiceProcessStepCommand(id));
        return result.ToEnvelopedResult(httpContext);
    }
}
