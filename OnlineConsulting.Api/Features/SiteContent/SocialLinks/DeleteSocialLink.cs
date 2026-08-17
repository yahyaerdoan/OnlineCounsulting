using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.SocialLinks.DeleteSocialLink;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.SocialLinks;

public class DeleteSocialLink : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/site-content/social-links/{id:guid}", Handle)
            .WithTags("SiteContent/SocialLinks")
            .RequireAuthorization()
            .WithName("DeleteSocialLink")
            .WithDescription("Deletes a site-wide social link.");
    }

    private static async Task<IResult> Handle(Guid id, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new DeleteSocialLinkCommand(id));
        return result.ToEnvelopedResult(httpContext);
    }
}
