using Core.PersistenceLayer.Dynamics.Dynamic;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Services.Application.Features.GetAllServicesPaged;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Services;

public class GetAllServicesPaged : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/services/query", Handle)
            .WithTags("Services")
            .WithName("GetAllServicesPaged")
            .WithDescription("Returns services, paginated (?index=&size=), optionally filtered/sorted via a DynamicQuery body.");
    }

    private static async Task<IResult> Handle(ISender sender, LinkGenerator linkGenerator, HttpContext httpContext, [AsParameters] ListQueryParameters query, [FromBody] DynamicQuery? dynamicQuery)
    {
        var result = await sender.Send(new GetAllServicesPagedQuery(query.ToPageRequest(), dynamicQuery));
        return result
            .OnSuccess(page =>
            {
                foreach (var service in page.Items)
                {
                    service.Links = GetServiceById.BuildLinks(httpContext, linkGenerator, service.Id);
                }
            })
            .ToEnvelopedResult(httpContext);
    }
}
