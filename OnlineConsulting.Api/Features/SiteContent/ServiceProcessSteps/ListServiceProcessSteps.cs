using Core.PersistenceLayer.Dynamics.Dynamic;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceProcessSteps.Contracts;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceProcessSteps.ListServiceProcessSteps;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.ServiceProcessSteps;

public class ListServiceProcessSteps : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/site-content/service-process-steps/query", Handle)
            .WithTags("SiteContent/ServiceProcessSteps")
            .WithName("ListServiceProcessSteps")
            .WithDescription("Returns service process steps, paginated (?index=&size=), optionally filtered/sorted via a DynamicQuery body.")
            .ProducesEnveloped<Paginate<ServiceProcessStepResponse>>();
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext, [AsParameters] ListQueryParameters query, [FromBody] DynamicQueryRequest? dynamicQuery)
    {
        var result = await sender.Send(new ListServiceProcessStepsQuery(query.ToPageRequest(), dynamicQuery?.ToDynamicQuery()));
        return result.ToEnvelopedResult(httpContext);
    }
}
