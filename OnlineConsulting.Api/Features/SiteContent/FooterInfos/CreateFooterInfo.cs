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
        _ = app.MapPost("/site-content/footer-info", Handle)
            .WithTags("SiteContent/FooterInfo")
            .RequireAuthorization()
            .WithName("CreateFooterInfo")
            .WithDescription("Creates a footer content block.");
    }

    private static async Task<IResult> Handle([FromBody] CreateFooterInfoRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToEnvelopedResult(httpContext);
    }
}

public record CreateFooterInfoRequest(string ImageUrl, string Description, int DisplayOrder = 0, Dictionary<string, object>? Metadata = null)
{
    public CreateFooterInfoCommand ToCommand() => new(ImageUrl, Description, DisplayOrder, Metadata);
}
