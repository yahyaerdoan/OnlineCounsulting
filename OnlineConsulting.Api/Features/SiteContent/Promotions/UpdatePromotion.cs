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
        _ = app.MapPut("/site-content/promotions/{id:guid}", Handle)
            .WithTags("SiteContent/Promotions")
            .RequireAuthorization()
            .WithName("UpdatePromotion")
            .WithDescription("Updates a promotional offer/CTA.")
            .ProducesEnveloped();
    }

    private static async Task<IResult> Handle(Guid id, [FromBody] UpdatePromotionRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand(id));
        return result.ToEnvelopedResult(httpContext);
    }
}

public record UpdatePromotionRequest(string Title, string Description, string? CtaText, string? CtaUrl, DateTimeOffset? ExpiresAt, int DisplayOrder = 0)
{
    public UpdatePromotionCommand ToCommand(Guid id) => new(id, Title, Description, CtaText, CtaUrl, ExpiresAt, DisplayOrder);
}
