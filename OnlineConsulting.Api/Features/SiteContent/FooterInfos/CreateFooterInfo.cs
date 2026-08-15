using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.FooterInfos.CreateFooterInfo;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.FooterInfos;

public class CreateFooterInfo : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/site-content/footer-info", Handle)
            .WithTags("SiteContent/FooterInfo")
            .RequireAuthorization()
            .WithName("CreateFooterInfo")
            .WithDescription("Creates a footer content block.");
    }

    private static async Task<IResult> Handle([FromBody] CreateFooterInfoCommand command, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(command);
        return result.ToEnvelopedResult(httpContext);
    }
}
