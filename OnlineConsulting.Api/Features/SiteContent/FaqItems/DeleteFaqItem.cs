using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.FaqItems.DeleteFaqItem;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.FaqItems;

public class DeleteFaqItem : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapDelete("/api/site-content/faq-items/{id:guid}", Handle)
            .WithTags("SiteContent/FaqItems")
            .RequireAuthorization()
            .WithName("DeleteFaqItem")
            .WithDescription("Deletes a service-specific FAQ item.");
    }

    private static async Task<IResult> Handle(Guid id, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new DeleteFaqItemCommand(id));
        return result.ToEnvelopedResult(httpContext);
    }
}
