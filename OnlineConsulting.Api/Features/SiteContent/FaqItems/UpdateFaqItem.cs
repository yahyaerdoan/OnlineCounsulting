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
        _ = app.MapPut("/site-content/faq-items/{id:guid}", Handle)
            .WithTags("SiteContent/FaqItems")
            .RequireAuthorization()
            .WithName("UpdateFaqItem")
            .WithDescription("Updates a service-specific FAQ item.");
    }

    private static async Task<IResult> Handle(Guid id, [FromBody] UpdateFaqItemRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand(id));
        return result.ToEnvelopedResult(httpContext);
    }
}

public record UpdateFaqItemRequest(Guid ServiceId, string Question, string Answer, int DisplayOrder = 0)
{
    public UpdateFaqItemCommand ToCommand(Guid id) => new(id, ServiceId, Question, Answer, DisplayOrder);
}
