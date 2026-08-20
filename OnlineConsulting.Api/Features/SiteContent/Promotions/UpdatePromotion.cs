using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.Promotions.UpdatePromotion;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.Promotions;

public class UpdatePromotion : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/api/site-content/promotions/{id:guid}", Handle)
            .WithTags("SiteContent/Promotions")
            .RequireAuthorization()
            .WithName("UpdatePromotion")
            .WithDescription("Updates a promotional offer/CTA.");
    }

    private static async Task<IResult> Handle(Guid id, [FromBody] UpdatePromotionCommand command, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(command with { Id = id });
        return result.ToEnvelopedResult(httpContext);
    }
}
