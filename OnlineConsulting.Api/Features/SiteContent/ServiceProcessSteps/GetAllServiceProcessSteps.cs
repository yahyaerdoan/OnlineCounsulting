using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceProcessSteps.GetAllServiceProcessSteps;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.ServiceProcessSteps;

public class GetAllServiceProcessSteps : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/site-content/service-process-steps", Handle)
            .WithTags("SiteContent/ServiceProcessSteps")
            .WithName("GetAllServiceProcessSteps")
            .WithDescription("Returns the \"how you get our service\" steps. Public - no login required.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new GetAllServiceProcessStepsQuery());
        return result.ToEnvelopedResult(httpContext);
    }
}
