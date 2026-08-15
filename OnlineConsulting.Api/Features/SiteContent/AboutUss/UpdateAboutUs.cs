using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.AboutUss.UpdateAboutUs;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.AboutUss;

public class UpdateAboutUs : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/site-content/about-us/{id:guid}", Handle)
            .WithTags("SiteContent/AboutUs")
            .RequireAuthorization()
            .WithName("UpdateAboutUs")
            .WithDescription("Updates an About Us content block.");
    }

    private static async Task<IResult> Handle(Guid id, [FromBody] UpdateAboutUsCommand command, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(command with { Id = id });
        return result.ToEnvelopedResult(httpContext);
    }
}
