using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.SocialLinks.CreateSocialLink;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.SocialLinks;

public class CreateSocialLink : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/site-content/social-links", Handle)
            .WithTags("SiteContent/SocialLinks")
            .RequireAuthorization()
            .WithName("CreateSocialLink")
            .WithDescription("Creates a site-wide social link.");
    }

    private static async Task<IResult> Handle([FromBody] CreateSocialLinkCommand command, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(command);
        return result.ToEnvelopedResult(httpContext);
    }
}
