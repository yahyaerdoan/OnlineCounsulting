using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.Promotions.DeletePromotion;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.Promotions;

public class DeletePromotion : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/site-content/promotions/{id:guid}", Handle)
            .WithTags("SiteContent/Promotions")
            .RequireAuthorization()
            .WithName("DeletePromotion")
            .WithDescription("Deletes a promotional offer/CTA.");
    }

    private static async Task<IResult> Handle(Guid id, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new DeletePromotionCommand(id));
        return result.ToEnvelopedResult(httpContext);
    }
}
