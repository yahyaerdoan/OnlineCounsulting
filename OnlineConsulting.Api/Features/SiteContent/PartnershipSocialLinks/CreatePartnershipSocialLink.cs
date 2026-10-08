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
        _ = app.MapPost("/site-content/partnership-social-links", Handle)
            .WithTags("SiteContent/PartnershipSocialLinks")
            .RequireAuthorization()
            .WithName("CreatePartnershipSocialLink")
            .WithDescription("Adds a social link to a partnership showcase entry.")
            .ProducesEnveloped<Guid>(StatusCodes.Status201Created);
    }

    private static async Task<IResult> Handle([FromBody] CreatePartnershipSocialLinkRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToEnvelopedResult(httpContext);
    }
}

public record CreatePartnershipSocialLinkRequest(Guid PartnershipId, string Name, string Url, string Icon, string? IconColor = null)
{
    public CreatePartnershipSocialLinkCommand ToCommand() => new(PartnershipId, Name, Url, Icon, IconColor);
}
