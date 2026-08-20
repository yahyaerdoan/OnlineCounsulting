using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.PartnershipSocialLinks.CreatePartnershipSocialLink;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.PartnershipSocialLinks;

public class CreatePartnershipSocialLink : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/site-content/partnership-social-links", Handle)
            .WithTags("SiteContent/PartnershipSocialLinks")
            .RequireAuthorization()
            .WithName("CreatePartnershipSocialLink")
            .WithDescription("Adds a social link to a partnership showcase entry.");
    }

    private static async Task<IResult> Handle([FromBody] CreatePartnershipSocialLinkCommand command, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(command);
        return result.ToEnvelopedResult(httpContext);
    }
}
