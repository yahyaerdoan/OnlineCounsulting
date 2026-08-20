using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.PartnershipSocialLinks.UpdatePartnershipSocialLink;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.PartnershipSocialLinks;

public class UpdatePartnershipSocialLink : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/api/site-content/partnership-social-links/{id:guid}", Handle)
            .WithTags("SiteContent/PartnershipSocialLinks")
            .RequireAuthorization()
            .WithName("UpdatePartnershipSocialLink")
            .WithDescription("Updates a partnership showcase entry's social link.");
    }

    private static async Task<IResult> Handle(Guid id, [FromBody] UpdatePartnershipSocialLinkCommand command, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(command with { Id = id });
        return result.ToEnvelopedResult(httpContext);
    }
}
