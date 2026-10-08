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
        _ = app.MapPut("/site-content/about-us/{id:guid}", Handle)
            .WithTags("SiteContent/AboutUs")
            .RequireAuthorization()
            .WithName("UpdateAboutUs")
            .WithDescription("Updates an About Us content block.")
            .ProducesEnveloped();
    }

    private static async Task<IResult> Handle(Guid id, [FromBody] UpdateAboutUsRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand(id));
        return result.ToEnvelopedResult(httpContext);
    }
}

public record UpdateAboutUsRequest(string Title, string Description, string? CoverImage, string? VideoUrl, int DisplayOrder = 0, Dictionary<string, object>? Metadata = null)
{
    public UpdateAboutUsCommand ToCommand(Guid id) => new(id, Title, Description, CoverImage, VideoUrl, DisplayOrder, Metadata);
}
