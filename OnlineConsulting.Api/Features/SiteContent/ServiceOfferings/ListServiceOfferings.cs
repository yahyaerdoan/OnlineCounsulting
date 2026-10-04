using Core.PersistenceLayer.Dynamics.Dynamic;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceOfferings.ListServiceOfferings;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.ServiceOfferings;

public class ListServiceOfferings : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/site-content/service-offerings/query", Handle)
            .WithTags("SiteContent/ServiceOfferings")
            .WithName("ListServiceOfferings")
            .WithDescription("Returns service offerings, paginated (?index=&size=), optionally filtered/sorted via a DynamicQuery body.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext, [AsParameters] ListQueryParameters query, [FromBody] DynamicQuery? dynamicQuery)
    {
        var result = await sender.Send(new ListServiceOfferingsQuery(query.ToPageRequest(), dynamicQuery));
        return result.ToEnvelopedResult(httpContext);
    }
}
