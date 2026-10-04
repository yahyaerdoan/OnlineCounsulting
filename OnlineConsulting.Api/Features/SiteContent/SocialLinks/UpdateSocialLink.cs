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
        _ = app.MapPut("/site-content/social-links/{id:guid}", Handle)
            .WithTags("SiteContent/SocialLinks")
            .RequireAuthorization()
            .WithName("UpdateSocialLink")
            .WithDescription("Updates a site-wide social link.");
    }

    private static async Task<IResult> Handle(Guid id, [FromBody] UpdateSocialLinkRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand(id));
        return result.ToEnvelopedResult(httpContext);
    }
}

public record UpdateSocialLinkRequest(string Name, string Url, string Icon, string? IconColor = null, int DisplayOrder = 0)
{
    public UpdateSocialLinkCommand ToCommand(Guid id) => new(id, Name, Url, Icon, IconColor, DisplayOrder);
}
