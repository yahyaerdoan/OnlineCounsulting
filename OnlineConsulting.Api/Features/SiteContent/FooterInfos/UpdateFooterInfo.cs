using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.FooterInfos.UpdateFooterInfo;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.FooterInfos;

public class UpdateFooterInfo : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/site-content/footer-info/{id:guid}", Handle)
            .WithTags("SiteContent/FooterInfo")
            .RequireAuthorization()
            .WithName("UpdateFooterInfo")
            .WithDescription("Updates a footer content block.")
            .ProducesEnveloped();
    }

    private static async Task<IResult> Handle(Guid id, [FromBody] UpdateFooterInfoRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand(id));
        return result.ToEnvelopedResult(httpContext);
    }
}

public record UpdateFooterInfoRequest(string ImageUrl, string Description, int DisplayOrder = 0, Dictionary<string, object>? Metadata = null)
{
    public UpdateFooterInfoCommand ToCommand(Guid id) => new(id, ImageUrl, Description, DisplayOrder, Metadata);
}
