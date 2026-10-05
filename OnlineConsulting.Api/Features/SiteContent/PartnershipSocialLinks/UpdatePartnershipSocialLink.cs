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
        _ = app.MapPut("/site-content/partnership-social-links/{id:guid}", Handle)
            .WithTags("SiteContent/PartnershipSocialLinks")
            .RequireAuthorization()
            .WithName("UpdatePartnershipSocialLink")
            .WithDescription("Updates a partnership showcase entry's social link.")
            .ProducesEnveloped();
    }

    private static async Task<IResult> Handle(Guid id, [FromBody] UpdatePartnershipSocialLinkRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand(id));
        return result.ToEnvelopedResult(httpContext);
    }
}

public record UpdatePartnershipSocialLinkRequest(string Name, string Url, string Icon, string? IconColor = null)
{
    public UpdatePartnershipSocialLinkCommand ToCommand(Guid id) => new(id, Name, Url, Icon, IconColor);
}
