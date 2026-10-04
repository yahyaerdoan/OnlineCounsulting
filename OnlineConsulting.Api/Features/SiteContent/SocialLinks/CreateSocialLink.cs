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
        _ = app.MapPost("/site-content/social-links", Handle)
            .WithTags("SiteContent/SocialLinks")
            .RequireAuthorization()
            .WithName("CreateSocialLink")
            .WithDescription("Creates a site-wide social link.");
    }

    private static async Task<IResult> Handle([FromBody] CreateSocialLinkRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToEnvelopedResult(httpContext);
    }
}

public record CreateSocialLinkRequest(string Name, string Url, string Icon, string? IconColor = null, int DisplayOrder = 0)
{
    public CreateSocialLinkCommand ToCommand() => new(Name, Url, Icon, IconColor, DisplayOrder);
}
