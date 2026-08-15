using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.AboutUss.CreateAboutUs;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.AboutUss;

public class CreateAboutUs : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/site-content/about-us", Handle)
            .WithTags("SiteContent/AboutUs")
            .RequireAuthorization()
            .WithName("CreateAboutUs")
            .WithDescription("Creates an About Us content block.");
    }

    private static async Task<IResult> Handle([FromBody] CreateAboutUsCommand command, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(command);
        return result.ToEnvelopedResult(httpContext);
    }
}
