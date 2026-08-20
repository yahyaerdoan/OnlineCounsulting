using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.AboutUss.DeleteAboutUs;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.AboutUss;

public class DeleteAboutUs : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapDelete("/api/site-content/about-us/{id:guid}", Handle)
            .WithTags("SiteContent/AboutUs")
            .RequireAuthorization()
            .WithName("DeleteAboutUs")
            .WithDescription("Deletes an About Us content block.");
    }

    private static async Task<IResult> Handle(Guid id, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new DeleteAboutUsCommand(id));
        return result.ToEnvelopedResult(httpContext);
    }
}
