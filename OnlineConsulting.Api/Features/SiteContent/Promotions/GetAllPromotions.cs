using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.Promotions.GetAllPromotions;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.Promotions;

public class GetAllPromotions : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/site-content/promotions", Handle)
            .WithTags("SiteContent/Promotions")
            .WithName("GetAllPromotions")
            .WithDescription("Returns the tenant's promotional offers. Public - no login required.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new GetAllPromotionsQuery());
        return result.ToEnvelopedResult(httpContext);
    }
}
