using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.FooterInfos.GetAllFooterInfos;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.FooterInfos;

public class GetAllFooterInfos : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/site-content/footer-info", Handle)
            .WithTags("SiteContent/FooterInfo")
            .WithName("GetAllFooterInfos")
            .WithDescription("Returns the tenant's footer content blocks. Public - no login required.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new GetAllFooterInfosQuery());
        return result.ToEnvelopedResult(httpContext);
    }
}
