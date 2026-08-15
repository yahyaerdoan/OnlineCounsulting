using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceOfferings.GetAllServiceOfferings;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.ServiceOfferings;

public class GetAllServiceOfferings : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/site-content/service-offerings", Handle)
            .WithTags("SiteContent/ServiceOfferings")
            .WithName("GetAllServiceOfferings")
            .WithDescription("Returns the \"what we provide\" cards. Public - no login required.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new GetAllServiceOfferingsQuery());
        return result.ToEnvelopedResult(httpContext);
    }
}
