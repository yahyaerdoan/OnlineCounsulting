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
        _ = app.MapPost("/site-content/about-us", Handle)
            .WithTags("SiteContent/AboutUs")
            .RequireAuthorization()
            .WithName("CreateAboutUs")
            .WithDescription("Creates an About Us content block.")
            .ProducesEnveloped<Guid>(StatusCodes.Status201Created);
    }

    private static async Task<IResult> Handle([FromBody] CreateAboutUsRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToEnvelopedResult(httpContext);
    }
}

public record CreateAboutUsRequest(string Title, string Description, string? CoverImage, string? VideoUrl, int DisplayOrder = 0, Dictionary<string, object>? Metadata = null)
{
    public CreateAboutUsCommand ToCommand() => new(Title, Description, CoverImage, VideoUrl, DisplayOrder, Metadata);
}
