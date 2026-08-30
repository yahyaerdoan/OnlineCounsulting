using Core.PersistenceLayer.Dynamics.Dynamic;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.AboutUss.GetAllAboutUssPaged;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.AboutUss;

public class GetAllAboutUssPaged : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/site-content/about-us/query", Handle)
            .WithTags("SiteContent/AboutUs")
            .WithName("GetAllAboutUssPaged")
            .WithDescription("Returns About Us entries, paginated (?index=&size=), optionally filtered/sorted via a DynamicQuery body.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext, [AsParameters] ListQueryParameters query, [FromBody] DynamicQuery? dynamicQuery)
    {
        var result = await sender.Send(new GetAllAboutUssPagedQuery(query.ToPageRequest(), dynamicQuery));
        return result.ToEnvelopedResult(httpContext);
    }
}
