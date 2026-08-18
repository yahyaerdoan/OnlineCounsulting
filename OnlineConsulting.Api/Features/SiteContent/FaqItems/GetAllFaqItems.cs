using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.FaqItems.GetAllFaqItems;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.FaqItems;

public class GetAllFaqItems : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/site-content/faq-items", Handle)
            .WithTags("SiteContent/FaqItems")
            .WithName("GetAllFaqItems")
            .WithDescription("Returns FAQ items, optionally filtered to a single service. Public - no login required.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext, Guid? serviceId = null)
    {
        var result = await sender.Send(new GetAllFaqItemsQuery(serviceId));
        return result.ToEnvelopedResult(httpContext);
    }
}
