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
        _ = app.MapPost("/site-content/promotions", Handle)
            .WithTags("SiteContent/Promotions")
            .RequireAuthorization()
            .WithName("CreatePromotion")
            .WithDescription("Creates a promotional offer/CTA.");
    }

    private static async Task<IResult> Handle([FromBody] CreatePromotionRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToEnvelopedResult(httpContext);
    }
}

public record CreatePromotionRequest(string Title, string Description, string? CtaText, string? CtaUrl, DateTimeOffset? ExpiresAt, int DisplayOrder = 0)
{
    public CreatePromotionCommand ToCommand() => new(Title, Description, CtaText, CtaUrl, ExpiresAt, DisplayOrder);
}
