using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.SocialLinks.UpdateSocialLink;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.SocialLinks;

public class UpdateSocialLink : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/api/site-content/social-links/{id:guid}", Handle)
            .WithTags("SiteContent/SocialLinks")
            .RequireAuthorization()
            .WithName("UpdateSocialLink")
            .WithDescription("Updates a site-wide social link.");
    }

    private static async Task<IResult> Handle(Guid id, [FromBody] UpdateSocialLinkCommand command, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(command with { Id = id });
        return result.ToEnvelopedResult(httpContext);
    }
}
