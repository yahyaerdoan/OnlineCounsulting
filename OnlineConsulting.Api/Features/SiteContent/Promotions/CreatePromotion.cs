using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.Promotions.CreatePromotion;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.Promotions;

public class CreatePromotion : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/site-content/promotions", Handle)
            .WithTags("SiteContent/Promotions")
            .RequireAuthorization()
            .WithName("CreatePromotion")
            .WithDescription("Creates a promotional offer/CTA.");
    }

    private static async Task<IResult> Handle([FromBody] CreatePromotionCommand command, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(command);
        return result.ToEnvelopedResult(httpContext);
    }
}
