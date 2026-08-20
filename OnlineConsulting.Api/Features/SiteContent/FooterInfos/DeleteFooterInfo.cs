using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.FooterInfos.DeleteFooterInfo;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.FooterInfos;

public class DeleteFooterInfo : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapDelete("/api/site-content/footer-info/{id:guid}", Handle)
            .WithTags("SiteContent/FooterInfo")
            .RequireAuthorization()
            .WithName("DeleteFooterInfo")
            .WithDescription("Deletes a footer content block.");
    }

    private static async Task<IResult> Handle(Guid id, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new DeleteFooterInfoCommand(id));
        return result.ToEnvelopedResult(httpContext);
    }
}
