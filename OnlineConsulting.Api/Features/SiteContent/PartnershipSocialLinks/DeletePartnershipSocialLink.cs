using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.PartnershipSocialLinks.DeletePartnershipSocialLink;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.PartnershipSocialLinks;

public class DeletePartnershipSocialLink : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/site-content/partnership-social-links/{id:guid}", Handle)
            .WithTags("SiteContent/PartnershipSocialLinks")
            .RequireAuthorization()
            .WithName("DeletePartnershipSocialLink")
            .WithDescription("Deletes a partnership showcase entry's social link.");
    }

    private static async Task<IResult> Handle(Guid id, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new DeletePartnershipSocialLinkCommand(id));
        return result.ToEnvelopedResult(httpContext);
    }
}
