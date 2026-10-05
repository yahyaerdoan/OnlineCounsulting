using Core.PersistenceLayer.Dynamics.Dynamic;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceAreas.Contracts;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceAreas.ListServiceAreas;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.ServiceAreas;

public class ListServiceAreas : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/site-content/service-areas/query", Handle)
            .WithTags("SiteContent/ServiceAreas")
            .WithName("ListServiceAreas")
            .WithDescription("Returns service areas, paginated (?index=&size=), optionally filtered/sorted via a DynamicQuery body.")
            .ProducesEnveloped<Paginate<ServiceAreaResponse>>();
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext, [AsParameters] ListQueryParameters query, [FromBody] DynamicQueryRequest? dynamicQuery)
    {
        var result = await sender.Send(new ListServiceAreasQuery(query.ToPageRequest(), dynamicQuery?.ToDynamicQuery()));
        return result.ToEnvelopedResult(httpContext);
    }
}
