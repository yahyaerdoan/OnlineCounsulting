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
        _ = app.MapPut("/api/site-content/footer-info/{id:guid}", Handle)
            .WithTags("SiteContent/FooterInfo")
            .RequireAuthorization()
            .WithName("UpdateFooterInfo")
            .WithDescription("Updates a footer content block.");
    }

    private static async Task<IResult> Handle(Guid id, [FromBody] UpdateFooterInfoCommand command, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(command with { Id = id });
        return result.ToEnvelopedResult(httpContext);
    }
}
