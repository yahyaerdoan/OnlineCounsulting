using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.FaqItems.UpdateFaqItem;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.FaqItems;

public class UpdateFaqItem : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/site-content/faq-items/{id:guid}", Handle)
            .WithTags("SiteContent/FaqItems")
            .RequireAuthorization()
            .WithName("UpdateFaqItem")
            .WithDescription("Updates a service-specific FAQ item.");
    }

    private static async Task<IResult> Handle(Guid id, [FromBody] UpdateFaqItemCommand command, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(command with { Id = id });
        return result.ToEnvelopedResult(httpContext);
    }
}
